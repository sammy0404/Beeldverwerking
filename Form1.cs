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
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height *45;
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
            int[,] imageValues = ToGrayscale(Image);
            int[,] dilation = Dilation(imageValues);
            int[,] erosion = Erosion(imageValues);

            imageValues = SubtractImage(dilation, erosion);
            imageValues = Threshold(imageValues, 25);

            
                imageValues = Erosion(imageValues);
            imageValues = Dilation(imageValues);
            
            int n = 1;
            for (int i = 0; i < n;i++ )
            {
                imageValues = Dilation(imageValues);
                imageValues = Erosion(imageValues);
            }
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

        private int[,] Dilation(int[,] image)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    List<int> values = new List<int>();

                    values.Add(image[x, y]);                              // Get the pixel color at coordinate (x,y)

                    if (y + 1 < image.GetLength(1))
                        values.Add(image[x, y + 1]);
                    if (y - 1 >= 0)
                        values.Add(image[x, y - 1]);
                    if (x + 1 < image.GetLength(0))
                        values.Add(image[x + 1, y]);
                    if (x - 1 >= 0)
                        values.Add(image[x - 1, y]);

                    if (x - 1 >= 0 && y - 1 >= 0)
                        values.Add(image[x - 1, y - 1]);
                    if (x - 1 >= 0 && y + 1 < image.GetLength(1))
                        values.Add(image[x - 1, y + 1]);
                    if (x + 1 < image.GetLength(0) && y + 1 < image.GetLength(1))
                        values.Add(image[x + 1, y + 1]);
                    if (x + 1 < image.GetLength(0) && y - 1 >= 0)
                        values.Add(image[x + 1, y - 1]);
                    
                    newImage[x,y] = values.Max();
                    progressBar.PerformStep();
                }
            }
            return newImage;
        }

        private int[,] Erosion(int[,] image)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < InputImage.Size.Width; x++)
            {
                for (int y = 0; y < InputImage.Size.Height; y++)
                {
                    List<int> values = new List<int>();

                    values.Add(image[x, y]);                              // Get the pixel color at coordinate (x,y)

                    if (y + 1 < image.GetLength(1))
                        values.Add(image[x, y + 1]);
                    if (y - 1 >= 0)
                        values.Add(image[x, y - 1]);
                    if (x + 1 < image.GetLength(0))
                        values.Add(image[x + 1, y]);
                    if (x - 1 >= 0)
                        values.Add(image[x - 1, y]);

                    if (x - 1 >= 0 && y - 1 >= 0)
                        values.Add(image[x - 1, y - 1]);
                    if (x - 1 >= 0 && y + 1 < image.GetLength(1))
                        values.Add(image[x - 1, y + 1]);
                    if (x + 1 < image.GetLength(0) && y + 1 < image.GetLength(1))
                        values.Add(image[x + 1, y + 1]);
                    if (x + 1 < image.GetLength(0) && y - 1 >= 0)
                        values.Add(image[x + 1, y - 1]);

                    newImage[x, y] = values.Min();
                    progressBar.PerformStep();
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
    }
}
