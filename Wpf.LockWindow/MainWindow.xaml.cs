using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Wpf.LockWindow.KeyboardHookLib;
using Control = System.Windows.Controls.Control;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Wpf.LockWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                m_dbConnection = new SQLiteConnection("Data Source=WallPaper.db;Version=3;");//没有数据库则自动创建
                m_dbConnection.Open();
                string sql = "create table  if not exists ImageTable (Id integer PRIMARY KEY AUTOINCREMENT,ImageData BLOB, ImageName TEXT)";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                InitializeComponent();

                if (WallPaperChangeThread == null)
                {
                    WallPaperChangeThread = new Thread(WallPaperChange);
                    WallPaperChangeThread.Start();
                }
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                if (TimeChangeThread == null)
                {
                    TimeChangeThread = new Thread(TimeChange);
                    TimeChangeThread.Start();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"异常");
            }


        }



        Thread TimeChangeThread = null;
        private void WallPaperChange()
        {
            try
            {
                while (true)
                {
                    string sql = "";
                    //读取数据库二进制流文件转图片
                    SQLiteCommand cmd = new SQLiteCommand(sql, m_dbConnection);
                    if (m_dbConnection.State != ConnectionState.Open)
                    {
                        m_dbConnection.Open();
                    }
                    DataSet ds = new DataSet();
                    sql = "select * from ImageTable";
                    cmd.CommandText = sql;
                    SQLiteDataAdapter adp = new SQLiteDataAdapter(cmd);
                    adp.Fill(ds);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            byte[] picData = (byte[])dr[1];
                            MemoryStream ms = new MemoryStream(picData);
                            ms.Seek(0, System.IO.SeekOrigin.Begin);
                            img.StreamSource = ms;
                            img.EndInit();
                            this.WallPaper.Source = img;
                            img.Freeze();
                            ms.Dispose();

                            //this.WallPaper.Source = new BitmapImage(new Uri("C:\\Users\\EDY\\Downloads\\【哲风壁纸】金克斯-雨中美女.png"));
                        }));
                        
                        Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "异常");
            }

        }
        private void TimeChange()
        {
            try
            {
                while (true)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        this.DateTime.Text = System.DateTime.Now.ToString("MM月dd日  HH:mm:ss");
                        if (this.WindowState != WindowState.Minimized && _keyboardHook == null)
                        {
                            _keyboardHook = new KeyboardHookLib();
                            //把客户端委托函数传给键盘钩子类KeyBoardHookLib
                            _keyboardHook.InstallHook(this.Form1_KeyPress);
                        }
                        else if (this.WindowState == WindowState.Minimized && _keyboardHook != null)
                        {
                            //卸载钩子
                            _keyboardHook.UninstallHook();
                            _keyboardHook = null;
                        }
                    }));

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }

            
        }

        Thread WallPaperChangeThread = null;



        private void UnLockWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }
        }

        //数据库连接
        SQLiteConnection m_dbConnection;

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strFileName = "";
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "图像文件|*.png;*.jpg";
                ofd.ValidateNames = true; // 验证用户输入是否是一个有效的Windows文件名
                ofd.CheckFileExists = true; //验证路径的有效性
                ofd.CheckPathExists = true;//验证路径的有效性
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) //用户点击确认按钮，发送确认消息
                {
                    strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串
                }
                if (String.IsNullOrEmpty(strFileName))
                {
                    MessageBox.Show("文件为空", "错误");
                }
                else
                {
                    ImportImageHelper.Updata_SQL(m_dbConnection, strFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }

        }



        //鼠标是否按下
        bool _isMouseDown = false;
        //鼠标按下的位置
        Point _mouseDownPosition;
        //鼠标按下控件的初始位置
        Point _mouseDownStartPosition;
        //鼠标按下控件的位置
        Point _mouseDownControlPosition;
        //鼠标按下事件
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Control;
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(this);
            var transform = c.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                c.RenderTransform = transform;
            }
            _mouseDownControlPosition = new Point(transform.X, transform.Y);
            _mouseDownStartPosition = _mouseDownControlPosition;
            c.CaptureMouse();
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var c = sender as Control;
                var pos = e.GetPosition(this);
                var dp = pos - _mouseDownPosition;
                var transform = c.RenderTransform as TranslateTransform;
                //transform.X = _mouseDownControlPosition.X + dp.X;
                transform.Y = _mouseDownControlPosition.Y + dp.Y;
                if (4 * Math.Abs(transform.Y)  > Screen.PrimaryScreen.Bounds.Height)
                {
                    transform.Y = _mouseDownStartPosition.Y;
                    _isMouseDown = false;
                    c.ReleaseMouseCapture();
                    this.WindowState = WindowState.Minimized;
                }
            }
        }


        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Control;
            var transform = c.RenderTransform as TranslateTransform;
            transform.Y = _mouseDownStartPosition.Y;
            _isMouseDown = false;
            c.ReleaseMouseCapture();
        }

        private void Win_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.LWin || e.Key == Key.RWin || e.Key == Key.Tab || e.Key == Key.F4)
                {
                            return;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"异常");
            }
        }


        private void Win_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin || key == Key.Tab || key == Key.F4)
            {
                e.Handled = true;
            }
        }


        private void Win_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin || key == Key.Tab || key == Key.F4)
            {
                e.Handled = true;
            }
        }


        private KeyboardHookLib _keyboardHook = null;
        //客户端传递的委托函数
        private void Form1_KeyPress(KeyboardHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = true; //预设不拦截
            return;
        }
    }
}
