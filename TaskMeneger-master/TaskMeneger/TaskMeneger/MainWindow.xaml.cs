using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Threading;

namespace TaskMeneger
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsExit = false;

        List<Task> tasks;

        List<Thread> threads = new List<Thread>();

        public MainWindow()
        {
            InitializeComponent();
            typeSelect.Items.Add("Почта");
            typeSelect.Items.Add("Скачать файл");
            typeSelect.Items.Add("Перенести каталог");

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Icon.ico");
            ni.Visible = true;
            ni.Click += ClickNi;

            this.Closing += ThisClosing;

            using (TaskContext context = new TaskContext())
            {
                tasks = context.TaskDb.ToList();
            }            

            foreach (var i in tasks)
            {
                if (i.TaskName == "Email")
                {
                    threads.Add(new Thread(() => TaskAction.SendMessages(i)));
                }
                else if (i.TaskName == "Move")
                {
                    threads.Add(new Thread(() => TaskAction.MoveCaralog(i)));
                }
                else if (i.TaskName == "Download")
                {
                    threads.Add(new Thread(() => TaskAction.DownloadFile(i)));
                }
            }

            foreach (var i in threads)
            {
                i.Start();
            }

        }

        private void ClickNi(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        private void ThisClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsExit)
            {
                e.Cancel = true;
                this.Visibility = Visibility.Collapsed;
            }
            else
                e.Cancel = false;
        }

        private void ButtonClickExit(object sender, RoutedEventArgs e)
        {
            using (TaskContext context = new TaskContext())
            {
                var taskList = context.TaskDb.ToList();

                for (int i = 0; i < taskList.Count; i++) 
                {
                    taskList[i].LastStart = tasks[i].LastStart;
                }

                context.SaveChanges();
            }
            TaskAction.IsActiv = false;
                IsExit = true;
            this.Close();
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (typeSelect.SelectedIndex == 0)
            {
                gridEmail.Visibility = Visibility.Visible;
                gridDownload.Visibility = Visibility.Collapsed;
                gridMaveCatalog.Visibility = Visibility.Collapsed;
                gridClose.Visibility = Visibility.Collapsed;
            }
            else if (typeSelect.SelectedIndex == 1)
            {
                gridEmail.Visibility = Visibility.Collapsed;
                gridDownload.Visibility = Visibility.Visible;
                gridMaveCatalog.Visibility = Visibility.Collapsed;
                gridClose.Visibility = Visibility.Collapsed;
            }
            else if (typeSelect.SelectedIndex == 2)
            {
                gridEmail.Visibility = Visibility.Collapsed;
                gridDownload.Visibility = Visibility.Collapsed;
                gridMaveCatalog.Visibility = Visibility.Visible;
                gridClose.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridEmail.Visibility = Visibility.Collapsed;
                gridDownload.Visibility = Visibility.Collapsed;
                gridMaveCatalog.Visibility = Visibility.Collapsed;
                gridClose.Visibility = Visibility.Visible;
            }
        }

        private void ButtonAddDownloadClick(object sender, RoutedEventArgs e)
        {
            Task newTask = new Task();
            int security;

            int.TryParse(textBoxDay.Text, out security);
            newTask.Day = security;

            int.TryParse(textBoxHour.Text, out security);
            newTask.Hour = security;

            int.TryParse(textBoxMinute.Text, out security);
            newTask.Minute = security;

            newTask.TaskName = "Download";

            newTask.FromDownloadPath = textBoxDownloadPath.Text;
            newTask.NameFile = textBoxDownloadName.Text;

            newTask.LastStart = DateTime.Now.AddMinutes(newTask.Minute).AddDays(newTask.Day).AddHours(newTask.Hour);

            tasks.Add(newTask);

            using (TaskContext context = new TaskContext())
            {
                context.TaskDb.Add(newTask);

                context.SaveChanges();
            }
            threads.Add(new Thread(TaskAction.DownloadFile));
        }

        private void ButtonAddEmailClick(object sender, RoutedEventArgs e)
        {
            Task newTask = new Task();
            int security;

            int.TryParse(textBoxDay.Text,out security);
            newTask.Day = security;

            int.TryParse(textBoxHour.Text, out security);
            newTask.Hour = security;

            int.TryParse(textBoxMinute.Text, out security);
            newTask.Minute = security;

            newTask.TaskName = "Email";

            newTask.HeadMessage = textBoxHeadEmail.Text;
            newTask.BodyMessage = textBoxBodyEmail.Text;

            newTask.LastStart = DateTime.Now.AddMinutes(newTask.Minute).AddDays(newTask.Day).AddHours(newTask.Hour);            

            tasks.Add(newTask);

            using (TaskContext context = new TaskContext())
            {
                context.TaskDb.Add(newTask);

                context.SaveChanges();
            }
            threads.Add(new Thread(TaskAction.SendMessages));
        }

        private void ButtonAddCatalogClick(object sender, RoutedEventArgs e)
        {
            Task newTask = new Task();
            int security;

            int.TryParse(textBoxDay.Text, out security);
            newTask.Day = security;

            int.TryParse(textBoxHour.Text, out security);
            newTask.Hour = security;

            int.TryParse(textBoxMinute.Text, out security);
            newTask.Minute = security;

            newTask.TaskName = "Move";

            newTask.MovePath = textBoxMoveCatalogPath.Text;
            newTask.Catalog = textBoxMoveCatalogName.Text;

            newTask.LastStart = DateTime.Now.AddMinutes(newTask.Minute).AddDays(newTask.Day).AddHours(newTask.Hour);

            tasks.Add(newTask);

            using (TaskContext context = new TaskContext())
            {
                context.TaskDb.Add(newTask);

                context.SaveChanges();
            }
            threads.Add(new Thread(TaskAction.MoveCaralog));
        }
    }
}
