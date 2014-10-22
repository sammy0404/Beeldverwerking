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
                    pictureBox1.Image = (Image) InputImage;                 // Display input image
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
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height *1150;
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
            int[,] imageValues = FilterWhite(Image,210);
            //int[,] dilation = Dilation(imageValues,1);

            imageValues = Threshold(imageValues, 130);
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
                    values[x,y] = (int)(0.2126 * pixelColor.R + 0.7152 * pixelColor.G + 0.0722 * pixelColor.B);
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
                        for(int sy = -size; sy <= size; sy++)
                        {
                            if (x + sx > 0 && x + sx < image.GetLength(0) && y + sy > 0 && y + sy < image.GetLength(1))
                            {
                                values.Add(image[x + sx, y + sy]);
                                if(image[x + sx, y + sy] == 255)
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
                    newImage[x, y] = image1[x, y] - image2[x,y];
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
                    if(image[x,y] < thresholdValue)
                        image[x,y] = 0;
                    else
                        image[x,y] = 255;
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
                    newImage[x,y] = Math.Min(image1[x, y], image2[x, y]);
                    progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private List<Tuple<int,int,int>> FindCircles(int[,] image,int samplepoints, int minRadius, int maxRadius, int radiusStep, float threshold)
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
                            circles.Add(new Tuple<int, int, int>(x,y,radius));
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
                    int red = image[x,y].R;
                    int blue = image[x,y].B;
                    int green = image[x,y].G;

                    int average = (red + blue + green)/3;

                    //int totalDifference = (int)((Math.Abs(red-average) + Math.Abs(blue-average) + Math.Abs(green-average)) * (1-(Math.Pow((double)(red + blue + green)/765,2))));

                    if(Math.Abs(red-blue) <= threshold && Math.Abs(green-blue) <= threshold && Math.Abs(blue-red) <= threshold)                    
                        newImage[x,y]=0;
                    else
                        newImage[x,y]=255;
                }
            }

            return newImage;
        }
    }
}
