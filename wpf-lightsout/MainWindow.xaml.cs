using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace wpf_lightsout
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Boolean array which will serve to check if the buttons(lights) are on or off
        private bool[,] checkLights = new bool[10, 10];

        private int gridSize;
        public const int MaxGridSize = 5;
        public const int MinGridSize = 3;
        public int GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {

                        if (value >= MinGridSize && value <= MaxGridSize)
                        {
                            gridSize = value;
                            checkLights = new bool[gridSize, gridSize];
                            AddLights();
                        }
 
            }
        }

        public void GenerateLights()
        {
            int rectSize = 5;

            //Loop through lights array declared above and generate buttons
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.White;
                    rect.Width = 10;
                    rect.Height = 10;
                    rect.Stroke = Brushes.Black;
                    // Store each row and col as a Point
                    rect.Tag = new Point(i, j);
                    // Register event handler
                    rect.MouseLeftButtonDown += ClickLight;
                    // Put the rectangle at the proper location within the canvas
                    Canvas.SetTop(rect, i * rectSize);
                    Canvas.SetLeft(rect, j * rectSize);
                    // Add the new rectangle to the canvas' children
                    MyCanvas.Children.Add(rect);
                }
            }
            AddLights();
        }

        public void AddLights()
        {
            using StreamReader r = new StreamReader("levels.json");

            string json = r.ReadToEnd();
            var levels = JsonConvert.DeserializeObject<List<Level>>(json);
            Random rand = new Random();

            foreach (var level in levels)
            {
                if (level.Id == ComboBox1.Items.IndexOf(ComboBox1.SelectedItem))
                {
                    for (int i = 0; i < level.Rows; i++)
                    {
                        for (int j = 0; j < level.Columns; j++)
                        {
                            checkLights[i, j] = rand.Next(2) == 1;
                        }
                    }
                }
            }
        }

        private void ClickLight(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            var rowCol = (Point)rect.Tag;
            int row = (int)rowCol.X;
            int col = (int)rowCol.Y;
            Move(row, col);
            DrawGrid();

            if (EndGame())
            {
                MessageBox.Show("You've won!", "You Win!");
            }
        }

        public bool GetGridValue(int row, int col)
        {
            return checkLights[row, col];
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set the colors of the rectangles
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    Rectangle rect = MyCanvas.Children[index] as Rectangle;
                    index++;
                    if (GetGridValue(r, c))
                    {
                        // On
                        rect.Fill = Brushes.White;
                        rect.Stroke = Brushes.Black;
                    }
                    else
                    {
                        // Off
                        rect.Fill = Brushes.Black;
                        rect.Stroke = Brushes.White;
                    }
                }
            }
        }

        public void Move(int row, int col)
        {
            if (row < 0 || row >= GridSize || col < 0 || col >= GridSize)
            {
                throw new ArgumentException("Row or column is outside the legal range of 0 to "
                + (GridSize - 1));
            }
            // Invert selected box and all surrounding boxes
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < GridSize && j >= 0 && j < GridSize)
                    {
                        checkLights[i, j] = !checkLights[i, j];
                    }
                }
            }
        }


        public bool EndGame()
        {
            //Loop through bool array, to check if lights are on or off
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    // If any light is on, the game still continues
                    // Else the game has finished
                    if (checkLights[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
            
        }



        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateLights();
            DrawGrid();
        }


        #region Helper
        //public void ClickLight(object sender, EventArgs e)
        //{
        //    Button light = sender as Button;
        //    //Get the index of the button clicked
        //    int i = (int)Char.GetNumericValue(light.Name[0]);
        //    int j = (int)Char.GetNumericValue(light.Name[1]);

        //    //Change value of clicked button
        //    ChangeButtonValueHandler(lights[i, j], i, j);

        //    //Check if game has finished
        //    EndGame();
        //}


        //public void ChangeButtonValueHandler(object sender, int i, int j)
        //{
        //    //Change value of the clicked button
        //    ChangeButtonValue(lights[i, j], i, j);

        //    //Change the value of the correct buttons around the clicked one
        //    if (i > 0)
        //    {
        //        ChangeButtonValue(lights[i - 1, j], i - 1, j);
        //    }
        //    if (i < (lights.GetLength(1) - 1))
        //    {
        //        ChangeButtonValue(lights[i + 1, j], i + 1, j);
        //    }
        //    if (j > 0)
        //    {
        //        ChangeButtonValue(lights[i, j - 1], i, j - 1);
        //    }
        //    if (j < (lights.GetLength(1) - 1))
        //    {
        //        ChangeButtonValue(lights[i, j + 1], i, j + 1);
        //    }

        //}

        //public void ChangeButtonValue(object sender, int i, int j)
        //{
        //    Button b = sender as Button;

        //    //Change the boolean value of the button if it's on or off
        //    checkLights[i, j] = !checkLights[i, j];

        //    //Set colours of the buttons(lights)
        //    if (checkLights[i, j] == true)
        //    {
        //        b.Background = Brushes.Yellow;
        //    }
        //    else
        //    {
        //        b.Background = Brushes.Black;
        //    }

        //}

        //public void EndGame()
        //{
        //    //Loop through bool array, to check if lights are on or off
        //    for (int i = 0; i < checkLights.GetLength(1); i++)
        //    {
        //        for (int j = 0; j < checkLights.GetLength(0); j++)
        //        {
        //            // If any light is on, the game still continues
        //            // Else the game has finished
        //            if (checkLights[i, j] == true)
        //            {
        //                return;
        //            }
        //        }
        //    }

        //    MessageBox.Show("You won!", "Press ok to exit!");
        //}
        #endregion
    }
}
