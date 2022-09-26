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
using Label = System.Windows.Controls.Label;
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
        private bool[,] checkLights = new bool[20, 20];

        public int GridSize()
        {
            using StreamReader r = new StreamReader("levels.json");

            string json = r.ReadToEnd();
            var levels = JsonConvert.DeserializeObject<List<Level>>(json);

            foreach (var level in levels)
            {
                if (level.Id == ComboBox1.Items.IndexOf(ComboBox1.SelectedItem))
                {
                    var gridSize = level.Rows;
                    return gridSize;
                }
            }
            return 0;
        }


        public void GenerateLights()
        {
            int rectSize = 30;

            //Loop through lights array declared above and generate buttons
            for (int i = 0; i < GridSize(); i++)
            {
                for (int j = 0; j < GridSize(); j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.White;
                    rect.Width = rectSize + 1;
                    rect.Height = rect.Width + 1;
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

        int count = 0;

        private void ClickLight(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            var rowCol = (Point)rect.Tag;
            int row = (int)rowCol.X;
            int col = (int)rowCol.Y;
            Move(row, col);
            DrawGrid();

            count++;
            if(rect != null)
            {
                MyTextbox.Text = count.ToString();
            }
            
            EndGame();
        }

        public bool GetGridValue(int row, int col)
        {
            return checkLights[row, col];
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set the colors of the rectangles
            for (int r = 0; r < GridSize(); r++)
            {
                for (int c = 0; c < GridSize(); c++)
                {
                    Rectangle rect = MyCanvas.Children[index] as Rectangle;
                    index++;
                    if (GetGridValue(r, c))
                    {
                        // On
                        rect.Fill = Brushes.Yellow;
                        rect.Stroke = Brushes.Black;
                    }
                    else
                    {
                        // Off
                        rect.Fill = Brushes.Black;
                        rect.Stroke = Brushes.Yellow;
                    }
                }
            }
        }

        public void Move(int row, int col)
        {
            if (row < 0 || row >= GridSize() || col < 0 || col >= GridSize())
            {
                throw new ArgumentException("Row or column is outside the legal range of 0 to "
                + (GridSize() - 1));
            }
            // Invert selected box and all surrounding boxes
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < GridSize() && j >= 0 && j < GridSize())
                    {
                        checkLights[i, j] = !checkLights[i, j];
                    }
                }
            }
        }


        public void EndGame()
        {
            //Loop through bool array, to check if lights are on or off
            for (int i = 0; i < GridSize(); i++)
            {
                for (int j = 0; j < GridSize(); j++)
                {
                    // If any light is on, the game still continues
                    // Else the game has finished
                    if (checkLights[i, j] == true)
                    {
                        return;
                    }
                }
            }

            MessageBox.Show("You won!", "Press ok to exit!");

            Restart();
        }


        public void Restart()
        {
            foreach (var item in MyMenu.Items)
            {
                if (item.GetType().Equals(typeof(TextBox)))
                {
                    MyTextbox.Text = String.Empty;
                }

                if (item.GetType().Equals(typeof(ComboBox)))
                {
                    ComboBox1.SelectedIndex = -1;
                }
            }
            MyCanvas.Children.Clear();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateLights();
            DrawGrid();
        }


        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }
    }
}
