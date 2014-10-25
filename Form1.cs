using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;
        Stack<Tuple<int, int>> bla = new Stack<Tuple<int, int>>();
        

        public INFOIBV()
        {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            if (openImageDialog.ShowDialog() == DialogResult.OK)             // Open File Dialog
            {
                string file = openImageDialog.FileName;                     // Get the file name
                imageFileName.Text = file;                                  // Show file name
                if (InputImage != null) InputImage.Dispose();               // Reset image
                InputImage = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image)InputImage;                 // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image
            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
            OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // Create new output image
            Color[,] Image = new Color[InputImage.Size.Width, InputImage.Size.Height]; // Create array to speed-up operations (Bitmap functions are very slow)

            // Setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height * 1150;
            progressBar.Value = 1;
            progressBar.Step = 1;

            // Copy input Bitmap to array            
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    Image[x, y] = InputImage.GetPixel(x, y);                // Set pixel color in array at (x,y)
                }
            }

            //==========================================================================================
            // TODO: include here your own code
            int[,] imageValues = FilterWhite(Image, 35);
            imageValues = Erosion(imageValues, 1);
            imageValues = Closing(imageValues, 4);

            PriorityQueue q = new PriorityQueue();

            q.Add(1, 2, 12);
            q.Add(1, 6, 11);
            q.Add(1, 10000, 10);
            q.Add(1, 1, 9);
            q.Add(1, 1, 8);
            q.Add(1, 1, 7);
            Console.WriteLine(q.ExtractMin().Item3);
            q.Add(1, 1, 6);
            q.Add(1, 1, 5);
            Console.WriteLine(q.ExtractMin().Item3);
            q.Add(1, 1, 3);
            q.Add(1, 1, 113);
            q.Add(1, 1, 2);
            Console.WriteLine(q.ExtractMin().Item3);
            q.Add(1, 1, 1);
            q.Add(1, 1, 0);
            q.Add(1, 1, -1);
            q.Add(1, 1, -2);

            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            Console.WriteLine(q.ExtractMin().Item3);
            //Console.WriteLine(q.ExtractMin().Item3);
            //Console.WriteLine(q.ExtractMin().Item3);
            //Console.WriteLine(q.ExtractMin().Item3);

            if (q.ExtractMin() == null)
                Console.WriteLine("done");
            else
                Console.WriteLine("error");
            //int[,] gray = ToGrayscale(Image);
            //int[,] erosionImage = Erosion(gray, 1);
            //int[,] dilationImage = Dilation(gray, 1);

            //int[,] imageValues = SubtractImage(dilationImage, erosionImage);
            //imageValues = Opening(imageValues, 2);
            //imageValues = FindObjects(imageValues);
            imageValues = ObjectDistance(imageValues);
            //imageValues = Threshold(imageValues, 10);
            //for (int x = 0; x < InputImage.Size.Width; x++)
            //{
             //   for (int y = 0; y < InputImage.Size.Height; y++)
              //  {
               //     imageValues[x, y] *= 10;
               // }
            //}

            //imageValues = Erosion(imageValues, 1);
            //imageValues = Dilation(imageValues,3);
            //imageValues = Opening(imageValues, 5);
            /*
            imageValues = SubtractImage(dilation, erosion);

            imageValues = Threshold(imageValues, 25);
            int[,] thresholdCopy = imageValues;

            //imageValues = Erosion(imageValues, 1);
            imageValues = Erosion(imageValues, 2);
            
            
            int n = 120;
            for (int i = 0; i < n; i++)
            {
               imageValues = Dilation(imageValues,1);
               imageValues = AND(imageValues, thresholdCopy);
            }*/
            //int[,] circleimage = new int[InputImage.Size.Width, InputImage.Size.Height];

            //List<Tuple<int, int, int>> circles = FindCircles(imageValues, 90, 50, 100, 1, 0.7f);
            //foreach(Tuple<int, int ,int> circle in circles)
            //{
            //    circleimage[circle.Item1, circle.Item2] = 255;
            // }
            //imageValues = circleimage;
            //imageValues = Closing(imageValues, 2);

            //==========================================================================================

            // Copy array to output Bitmap
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int grayValue = imageValues[x, y];
                    OutputImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));               // Set the pixel color at coordinate (x,y)
                }
            }

            pictureBox2.Image = (Image)OutputImage;                         // Display output image
            progressBar.Visible = false;                                    // Hide progress bar
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }

        private int[,] ToGrayscale(Color[,] image)
        {
            int[,] values = new int[image.GetLength(0), image.GetLength(1)];
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    Color pixelColor = image[x, y];                              // Get the pixel color at coordinate (x,y)
                    values[x, y] = (int)(0.2126 * pixelColor.R + 0.7152 * pixelColor.G + 0.0722 * pixelColor.B);
                    progressBar.PerformStep();                                   // Increment progress bar
                }
            }
            return values;
        }

        private int[,] Dilation(int[,] image, int size)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    List<int> values = new List<int>();

                    for (int sx = -size; sx <= size; sx++)
                    {
                        for (int sy = -size; sy <= size; sy++)
                        {
                            if (x + sx > 0 && x + sx < image.GetLength(0) && y + sy > 0 && y + sy < image.GetLength(1))
                            {
                                values.Add(image[x + sx, y + sy]);
                                if (image[x + sx, y + sy] == 255)
                                    goto DilationDone;
                            }
                        }
                    }
                DilationDone:
                    newImage[x, y] = values.Max();
                    //progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private int[,] Erosion(int[,] image, int size)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    List<int> values = new List<int>();

                    for (int sx = -size; sx <= size; sx++)
                    {
                        for (int sy = -size; sy <= size; sy++)
                        {
                            if (x + sx > 0 && x + sx < image.GetLength(0) && y + sy > 0 && y + sy < image.GetLength(1))
                            {
                                values.Add(image[x + sx, y + sy]);
                                if (image[x + sx, y + sy] == 0)
                                    goto ErosionDone;
                            }
                        }
                    }
                ErosionDone:
                    newImage[x, y] = values.Min();
                    //progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private int[,] SubtractImage(int[,] image1, int[,] image2)
        {
            int[,] newImage = new int[image1.GetLength(0), image1.GetLength(1)];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    newImage[x, y] = image1[x, y] - image2[x, y];
                    progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private int[,] Threshold(int[,] image, int thresholdValue)
        {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    if (image[x, y] < thresholdValue)
                        image[x, y] = 0;
                    else
                        image[x, y] = 255;
                    progressBar.PerformStep();
                }
            }
            return image;
        }

        private int[,] Opening(int[,] image, int size)
        {
            image = Erosion(image, size);
            image = Dilation(image, size);
            return image;
        }

        private int[,] Closing(int[,] image, int size)
        {
            image = Dilation(image, size);
            image = Erosion(image, size);
            return image;
        }

        private int[,] AND(int[,] image1, int[,] image2)
        {
            int[,] newImage = new int[image1.GetLength(0), image1.GetLength(1)];
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    newImage[x, y] = Math.Min(image1[x, y], image2[x, y]);
                    progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private List<Tuple<int, int, int>> FindCircles(int[,] image, int samplepoints, int minRadius, int maxRadius, int radiusStep, float threshold)
        {
            List<Tuple<int, int, int>> circles = new List<Tuple<int, int, int>>();
            float degreesperstep = 360 / samplepoints;

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {

                    for (int radius = minRadius; radius <= maxRadius; radius += radiusStep)
                    {
                        int counter = 0;
                        int misses = 0;
                        for (float degree = 0; degree <= 360; degree += degreesperstep)
                        {
                            int dx = (int)(Math.Cos((degree * Math.PI) / 180) * radius);
                            int dy = (int)(Math.Sin((degree * Math.PI) / 180) * radius);

                            int xpos = dx + x;
                            int ypos = dy + y;

                            if (xpos >= 0 && xpos < InputImage.Size.Width && ypos >= 0 && ypos < InputImage.Size.Height && image[xpos, ypos] > 0)
                                counter++;
                            else
                                misses++;

                            if (misses > samplepoints - (threshold * samplepoints))
                                break;

                        }

                        if (counter >= samplepoints * threshold)
                            circles.Add(new Tuple<int, int, int>(x, y, radius));
                    }
                }
            }
            return circles;
        }

        int[,] FilterWhite(Color[,] image, int threshold)
        {
            int[,] newImage = new int[InputImage.Size.Width, InputImage.Size.Height];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    int red = image[x, y].R;
                    int blue = image[x, y].B;
                    int green = image[x, y].G;

                    int average = (red + blue + green) / 3;

                    int totalDifference = (int)((Math.Abs(red - average) + Math.Abs(blue - average) + Math.Abs(green - average)) * (756f / (red + blue + green)));

                    if (totalDifference > threshold)
                        newImage[x, y] = 255;
                    else
                        newImage[x, y] = 0;
                }
            }

            return newImage;
        }

        int[,] FindObjects(int[,] image)
        {
            int[,] objectImage = new int[InputImage.Size.Width, InputImage.Size.Height];
            int foundObjects = 0;
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    if (image[x, y] == 255 && objectImage[x, y] == 0)
                    {

                        foundObjects++;
                        bla.Push(new Tuple<int, int>(x, y));
                        while (bla.Count > 0)
                        {
                            Tuple<int, int> coord = bla.Pop();
                            FindWholeObject(image, ref objectImage, coord.Item1, coord.Item2, foundObjects);
                        }

                    }
                }
            }
            return objectImage;
        }

        void FindWholeObject(int[,] image, ref int[,] objectImage, int x, int y, int objectNumber)
        {
            objectImage[x, y] = objectNumber;
            if (x - 1 >= 0 && y - 1 >= 0 && y - 1 < InputImage.Size.Height && InputImage.Size.Width > x - 1 && image[x - 1, y - 1] == 255 && objectImage[x - 1, y - 1] == 0)
                bla.Push(new Tuple<int, int>(x - 1, y - 1));
            if (x >= 0 && y - 1 >= 0 && y - 1 < InputImage.Size.Height && InputImage.Size.Width > x && image[x, y - 1] == 255 && objectImage[x, y - 1] == 0)
                bla.Push(new Tuple<int, int>(x, y - 1));
            if (InputImage.Size.Width > x + 1 && x + 1 >= 0 && y - 1 >= 0 && y - 1 < InputImage.Size.Height && image[x + 1, y - 1] == 255 && objectImage[x + 1, y] == 0)
                bla.Push(new Tuple<int, int>(x + 1, y - 1));
            if (InputImage.Size.Width > x - 1 && x - 1 >= 0 && y >= 0 && y < InputImage.Size.Height && image[x - 1, y] == 255 && objectImage[x - 1, y] == 0)
                bla.Push(new Tuple<int, int>(x - 1, y));
            if (InputImage.Size.Width > x + 1 && x + 1 >= 0 && y >= 0 && y < InputImage.Size.Height && image[x + 1, y] == 255 && objectImage[x + 1, y] == 0)
                bla.Push(new Tuple<int, int>(x + 1, y));
            if (InputImage.Size.Width > x - 1 && x - 1 >= 0 && y + 1 >= 0 && y + 1 < InputImage.Size.Height && image[x - 1, y + 1] == 255 && objectImage[x - 1, y + 1] == 0)
                bla.Push(new Tuple<int, int>(x - 1, y + 1));
            if (InputImage.Size.Width > x && x >= 0 && y + 1 >= 0 && y + 1 < InputImage.Size.Height && image[x, y + 1] == 255 && objectImage[x, y + 1] == 0)
                bla.Push(new Tuple<int, int>(x, y + 1));
            if (InputImage.Size.Width > x + 1 && x + 1 >= 0 && y + 1 >= 0 && y + 1 < InputImage.Size.Height && image[x + 1, y + 1] == 255 && objectImage[x + 1, y + 1] == 0)
                bla.Push(new Tuple<int, int>(x + 1, y + 1));
        }

        bool Equals(int[,] image1, int[,] image2)
        {
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    if (image1[x, y] != image2[x, y])
                        return false;
                }
            }
            return true;
        }

        int[,] ObjectDistance(int[,] image)
        {

            int[,] DistanceImage = new int[InputImage.Size.Width, InputImage.Size.Height];
            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    if (image[x, y] == 255)
                    {
                        DistanceImage[x, y] = int.MaxValue;
                    }
                }
            }
            for (int y = 0; y < InputImage.Size.Height; y++)
            {
                for (int x = 0; x < InputImage.Size.Width; x++)
                {
                    List<int> IntKernel = new List<int>();
                    if (x - 1 >= 0 && y - 1 >= 0)
                        IntKernel.Add(DistanceImage[x - 1, y - 1] + 2);
                    if (y - 1 >= 0)
                        IntKernel.Add(DistanceImage[x, y - 1] + 1);
                    if (x + 1 < InputImage.Size.Width && y - 1 >= 0)
                        IntKernel.Add(DistanceImage[x + 1, y - 1] + 2);
                    if (x - 1 >= 0)
                        IntKernel.Add(DistanceImage[x - 1, y] + 1);
                    IntKernel.Add(DistanceImage[x, y]);
                    DistanceImage[x, y] = IntKernel.Min();
                }
            }
            for (int y = InputImage.Size.Height-1; y >=0; y--)
            {
                for (int x = InputImage.Size.Width-1; x >=0; x--)
                {
                    List<int> IntKernel = new List<int>();
                    if (x - 1 >= 0 && y + 1 < InputImage.Size.Height)
                        IntKernel.Add(DistanceImage[x - 1, y + 1] + 2);
                    if (y + 1 < InputImage.Size.Height)
                        IntKernel.Add(DistanceImage[x, y + 1] + 1);
                    if (x + 1 < InputImage.Size.Width && y + 1 < InputImage.Size.Height)
                        IntKernel.Add(DistanceImage[x + 1, y + 1] + 2);
                    if (x + 1 < InputImage.Size.Width)
                        IntKernel.Add(DistanceImage[x + 1, y] + 1);
                    IntKernel.Add(DistanceImage[x, y]);
                    DistanceImage[x, y] = IntKernel.Min();
                }
            }
            return DistanceImage;
        }
    }

    class PriorityQueue
    {
        List<Tuple<int, int, int>> heap;
        int size;

        public PriorityQueue()
        {
            heap = new List<Tuple<int, int, int>>();
        }

        public void Add(int x, int y , int grayValue)
        {
            Tuple<int, int, int> newValue = new Tuple<int, int, int>(x, y, grayValue);

            heap.Add(newValue);
            size++;

            int currentPlace = size - 1;

            while(heap[Parent(currentPlace)].Item3 > heap[currentPlace].Item3 && currentPlace != 0)
            {
                Tuple<int, int, int> temp = heap[Parent(currentPlace)];
                heap[Parent(currentPlace)] = heap[currentPlace];
                heap[currentPlace] = temp;
                currentPlace = Parent(currentPlace);
            }
        }

        public Tuple<int, int, int> ExtractMin()
        {
            Tuple<int, int, int> min;

            if (size > 0)
                min = heap[0];
            else
                return null;

            Tuple<int, int, int> last = heap[size - 1];
            size--;
            heap[0] = last;
            int currentPlace =0;
            
            while(Left(currentPlace)<=size)
            {
                int smallest = currentPlace;

                if(heap[Left(currentPlace)].Item3 < heap[smallest].Item3)
                {
                    smallest = Left(currentPlace);
                }
                if(Right(currentPlace) <= size && heap[Right(currentPlace)].Item3 < heap[smallest].Item3)
                {
                    smallest = Right(currentPlace);
                }

                if (currentPlace == smallest)
                    break;

                Tuple<int, int, int> temp = heap[currentPlace];
                heap[currentPlace] = heap[smallest];
                heap[smallest] = temp;                
                currentPlace = smallest;                
            }
            return min;
        }

        private int Left(int i)
        {
            return i * 2 + 1;
        }

        private int Right(int i)
        {
            return i * 2 + 2;
        }

        private int Parent(int i)
        {
            return (i-1) / 2;
        }
    }
}
