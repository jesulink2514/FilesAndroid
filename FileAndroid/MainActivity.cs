using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.IO;

namespace FileAndroid
{
    [Activity(Label = "FileAndroid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private EditText _jokeText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            LoadJokesToFile();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var jokeButton = FindViewById<Button>(Resource.Id.chisteButton);
            var saveButton = FindViewById<Button>(Resource.Id.guardarButton);
            _jokeText = FindViewById<EditText>(Resource.Id.chisteText);

            jokeButton.Click += ReadJoke;
            saveButton.Click += SaveJoke;
        }

        private async void SaveJoke(object sender, EventArgs e)
        {
            var joke = _jokeText.Text;

            if(string.IsNullOrWhiteSpace(joke))return;

            var dbPath = Path.Combine(System.Environment
               .GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
               "jokes.txt");
            
            using (var stream = File.Open(dbPath,FileMode.Open))
            using(var writer = new StreamWriter(stream))
            {
              await writer.WriteLineAsync(joke);   
            }
                
            _jokeText.Text = string.Empty;
        }

        private void ReadJoke(object sender, EventArgs e)
        {
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"jokes.txt");
            var lines = File.ReadAllLines(dbPath);
            var linesCount = lines.Length;
            var jokeIndex = new Random(DateTime.Now.Millisecond).Next(0,linesCount-1);
            var joke= lines[jokeIndex];

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("File Sample");
            builder.SetMessage(joke);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK",delegate{ });
            Dialog dialog = builder.Create();
            dialog.Show();
            return;
        }

        private async void LoadJokesToFile()
        {
            var dbPath = Path.Combine(System.Environment
                .GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                "jokes.txt");

            if (File.Exists(dbPath)) return;


            using (var stream = Assets.Open("jokes.txt"))
            using (var reader = new StreamReader(stream))
            using (var writer = File.CreateText(dbPath))
            {
                string line;
                do
                {
                    line = await reader.ReadLineAsync();
                    if (line != null)
                        await writer.WriteLineAsync(line);
                } while (line != null);
            }
        }
    }
}