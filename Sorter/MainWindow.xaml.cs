using System;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace Sorter
{
    public partial class MainWindow : Window
    {
        private int MAX_HEIGHT = 400;
        private const int MAX_WIDTH = 800;
        private string sortType = "BULB";
        private int[] numbers;
        private Rectangle[] rectangles;
        private Thread sortingThread;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            if (sortingThread != null)
            {
                sortingThread.Abort();
            }
            Close();
        }

        private void RunSorting(object sender, MouseButtonEventArgs e)
        {
            RunSortingButton.IsEnabled = false;
            GetSortType();
            sortingThread = new Thread(delegate()
            {
                Sorting();
                PrintArrayToTextField(numbers);
            });
            sortingThread.Start();
        }

        private void GetSortType()  //выбор сортировки
        {
            InitButton.IsEnabled = false;
            if (FirstRadioButton.IsChecked == true) sortType = "BULB";
            if (SecondRadioButton.IsChecked == true) sortType = "INSERTION";
            if (ThirdRadioButton.IsChecked == true) sortType = "SELECTION";
        }

        private void Sorting()  //соответствующий алгоритм выбранной сортировки
        {
            if (sortType.Equals("BULB")) numbers = BulbSort(numbers);
            if (sortType.Equals("INSERTION")) numbers = InsertionSort(numbers);
            if (sortType.Equals("SELECTION")) numbers = SelectionSort(numbers);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                RunSortingButton.IsEnabled = true;
            });
        }

        //кнопка "зафиксировать"
        private void InitializeButton(object sender, MouseButtonEventArgs e)  
        {
            MAX_HEIGHT = 400;  //максимальное значение для элемента массива
            OutputTextBox.Text = "";
            numbers = GetNumbersFromTextField();
            InitializeRectangles(numbers); //первая зарисовка полученного массива
            RunSortingButton.IsEnabled = true;
            PrintMasLength(numbers);
        }

        private int[] GetNumbersFromTextField()  //из строки в массив
        {
            string input = InputTextBox.Text;
            if (input.Equals(""))
            {
                input = "8 3 2 4 6 9 7 6 3 5 7 2 1 4";
                InputTextBox.Text = input;
            }
            string[] array = input.Split(' ');
            int[] numbers = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Equals(""))
                {
                    numbers[i] = Int32.Parse(array[i]);
                }
            }
            return numbers;
        }

        //СОРТИРОВКА ПУЗЫРЕК
        private int[] BulbSort(int[] array)
        {
            long timeresult;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int temp;
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    SetOrangeColor(i, j);
                    if (array[j] < array[i])
                    {
                        temp = array[j];
                        array[j] = array[i];
                        array[i] = temp;
                        SwapRectangles(i, j);
                    }
                    sw.Stop();
                    Thread.Sleep(50);
                    sw.Start();
                    SetRedColor(i, j);
                }
                SetGreenColor(i);
            }
            sw.Stop();
            timeresult = sw.ElapsedMilliseconds;
            PrintTimerToTextbox(timeresult);
            return array;
        }
        
        //СОРТИРОВКА ВСТАВКАМИ
        public int[] InsertionSort(int[] array)
        {
            long timeresult;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int[] result = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                int j = i;
                while (j > 0 && result[j - 1] > array[i])
                {
                    SetOrangeColor(i, j - 1);
                    result[j] = result[j - 1];
                    SwapRectangles(j, j - 1);
                    sw.Stop();
                    Thread.Sleep(50);
                    sw.Start();
                    SetRedColor(i, j - 1);
                    j--;
                }
                result[j] = array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                SetGreenColor(i);
            }
            sw.Stop();
            timeresult = sw.ElapsedMilliseconds;
            PrintTimerToTextbox(timeresult);
            return result;
        }

        //СОРТИРОВКА ВЫБОРОМ
        public int[] SelectionSort(int[] array)
        {
            long timeresult;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int min, temp;
            for (int i = 0; i < array.Length - 1; i++)
            {
                min = i;

                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[j] < array[min])
                    {
                        min = j;
                    }
                }
                SetOrangeColor(i, min);
                if (min != i)
                {
                    temp = array[i];
                    array[i] = array[min];
                    array[min] = temp;
                    SwapRectangles(i, min);
                }
                sw.Stop();
                Thread.Sleep(50);
                sw.Start();
                SetRedColor(i, min);
                SetGreenColor(i);
            }
            SetGreenColor(array.Length - 1);
            sw.Stop();
            timeresult = sw.ElapsedMilliseconds;
            PrintTimerToTextbox(timeresult);
            return array;
        }

        private void PrintTimerToTextbox(long temptime)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                OutputTimer.Text = "";
                OutputTimer.Text = temptime.ToString();
              
            });
        }

        //вывод массива в textbox (поэлементно)
        private void PrintArrayToTextField(int[] array)  
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                OutputTextBox.Text = "";
                foreach (int n in array)
                {
                    OutputTextBox.Text += n + " ";
                }
                InitButton.IsEnabled = true;
            });
        }

        private void PrintMasLength(int[] array)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                int ml = array.Length;
                MasLength.Text = "";
                MasLength.Text = ml.ToString();

            });
        }

        //построение картины отрисовки массива
        private void InitializeRectangles(int[] array)  
        {
            if (HasNegative(array)) MAX_HEIGHT = MAX_HEIGHT / 2;  //проверка на наличие отрицательных элементов => установка max высоты

            int space = MAX_WIDTH / array.Length;
            int maxNumber = GetMax(array);
            int pixelsPerNumber = MAX_HEIGHT / maxNumber;
            DrawingCanvas.Children.Clear();
            rectangles = new Rectangle[array.Length];
            for (int i = 0; i < array.Length; i++)
            {   
                int width = 0;
                 if (array.Length < 21) { width = 25; }
                 else width = 15;
                if (array.Length > 30) { width = 10; }
                if (array.Length > 40) { width = 5; }
                int height = 0;
                int top = 0;
                 if (array[i] < 0) //проверка на наличие отрицательнх элементов и поиск середины
                 {  
                     height = (-1) * array[i] * pixelsPerNumber;
                     top = MAX_HEIGHT;
                 }
                 else
                 {
                     height = array[i] * pixelsPerNumber;
                     top = MAX_HEIGHT - height;
                 }
                int left = space * i;

                //описание свойств объекта
                Rectangle rectangle = new Rectangle();  //экземпляр класса рисующего прямоугольник
                SolidColorBrush myBrush = new SolidColorBrush(Colors.Red); //кисть
                rectangle.Fill = myBrush;  // задание способа зарисовки обектов (заливка)
                rectangle.Height = height;  //высота элемента(прямоугольника)
                rectangle.Width = width;  //ширина
                rectangle.Margin = new Thickness(left, top, 0, 0);
                rectangles[i] = rectangle;
                DrawingCanvas.Children.Add(rectangle); //добавляем элемент на область рисования (Canvas)
            }
        }

        private void SwapRectangles(int index1, int index2)  //меняем местами прямоугольники
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() //старт потока
            {
                int leftFirst = (int)rectangles[index1].Margin.Left;
                int leftSecond = (int)rectangles[index2].Margin.Left;
                int topFirst = (int)rectangles[index1].Margin.Top;
                int topSecond = (int)rectangles[index2].Margin.Top;
                rectangles[index1].Margin = new Thickness(leftSecond, topFirst, 0, 0);
                rectangles[index2].Margin = new Thickness(leftFirst, topSecond, 0, 0);
            });
            Rectangle temp = rectangles[index1];
            rectangles[index1] = rectangles[index2];
            rectangles[index2] = temp;
        }

        private void SetOrangeColor(int index1, int index2)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() //приоритет - Normal, делегат помещаем в очередь событий
            {
                rectangles[index1].Fill = new SolidColorBrush(Colors.Orange);  //определили способ заливки
                rectangles[index2].Fill = new SolidColorBrush(Colors.Orange);
            });
        }

        private void SetRedColor(int index1, int index2)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                rectangles[index1].Fill = new SolidColorBrush(Colors.Red);
                rectangles[index2].Fill = new SolidColorBrush(Colors.Red);
            });
        }
        private void SetGreenColor(int index1)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                rectangles[index1].Fill = new SolidColorBrush(Colors.Green);
            });
        }

        //Поиск максимального элемента
        private int GetMax(int[] array) 
        {
            int temp = array[0];
            foreach (int n in array)
            {
                if (n < 0)
                {
                    if (-1*n > temp) temp = -1*n;
                }
                if (n > temp) temp = n;
            }
            return temp;
        }

        //Проверка наличия отрицательных элементов
        private bool HasNegative(int[] array)
        {
            foreach (int n in array)
            {
                if (n < 0) return true;
            }
            return false;
        }
    }
}
