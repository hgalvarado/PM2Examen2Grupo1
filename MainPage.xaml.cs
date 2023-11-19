using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Maps;
using Plugin.Maui.Audio;
using System.IO;


namespace PM2Examen2Grupo1
{
    public partial class MainPage : ContentPage
    {
        string videoPath;

        //Audio
        private readonly IAudioRecorder _audioRecorder;
        private bool isRecording = false;
        public string pathaudio, filename;

        //obtener ubicacion Actual
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public MainPage()
        {
            InitializeComponent();
            _audioRecorder = AudioManager.Current.CreateRecorder();
            
        }

        public async Task GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    txtLatitud.Text = location.Latitude.ToString();
                    txtLongitud.Text = location.Longitude.ToString();
                    await DisplayAlert("Mensaje", "Ubicacion lista", "OK");
                }
                    

            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }



        public async Task TakePhotoAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult video = await MediaPicker.Default.CaptureVideoAsync();

                if (video != null)
                {
                    string localFolderPath = Path.Combine(FileSystem.CacheDirectory, "Videos");
                    Directory.CreateDirectory(localFolderPath); // Asegurarse de que la carpeta existe

                    string localFilePath = Path.Combine(localFolderPath, video.FileName);

                    using (Stream sourceStream = await video.OpenReadAsync())
                    using (FileStream localFileStream = File.Create(localFilePath)) // Use 'File.Create' to ensure a new copy
                    //using (FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }


                    // Suponiendo que `viewVideo` es un control MediaElement
                    viewVideo.Source = MediaSource.FromFile(localFilePath);
                }
            }
        }
















        private void OnCounterClicked(object sender, EventArgs e)
        {
           
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {

        }

        private async void btnGabarVideo_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
        }

        private string GetVideoPath(Stream stream)
        {
        #if __ANDROID__
                    var publicDirectoryPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                    var videoPath = Path.Combine(publicDirectoryPath, "video.mp4");

                    using (var fileStream = File.Create(videoPath))
                    {
                        stream.CopyTo(fileStream);
                    }

                    return videoPath;
        #else
                    return string.Empty;
        #endif
        }

        private async void btnObtenerUbicacion_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alerta","Obteniendo Ubicacion","OK");
            await GetCurrentLocation();
        }

        private async void btnGrabarAudio_Clicked(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                var permiso = await Permissions.RequestAsync<Permissions.Microphone>();
                var permiso1 = await Permissions.RequestAsync<Permissions.StorageRead>();
                var permiso2 = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (permiso != PermissionStatus.Granted || permiso1 != PermissionStatus.Granted || permiso2 != PermissionStatus.Granted)
                {
                    return;
                }
                await _audioRecorder.StartAsync();
                isRecording = true;
                btnGrabarAudio.Text = "Grabando";
                Console.WriteLine("Iniciando grabación...");
            }
            else
            {
                var recordedAudio = await _audioRecorder.StopAsync();

                if (recordedAudio != null)
                {
                    try
                    {
                        filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DateTime.Now.ToString("ddMMyyyymmss") + "_VoiceNote.wav");

                        using (var fileStorage = new FileStream(filename, FileMode.Create, FileAccess.Write))
                        {
                            recordedAudio.GetAudioStream().CopyTo(fileStorage);
                        }

                        pathaudio = filename;

                        //Audios nuevoAudio = new Audios
                        //{
                        //    descripcion = txtdescripcion.Text,
                        //    url = pathaudio,
                        //    fecha = DateTime.Now
                        //};

                        //int resultado = await App.BDAudios.GrabarAudio(nuevoAudio);

                        //if (resultado > 0)
                        //{
                        //    await DisplayAlert("", "Audio grabado correctamente", "Ok");
                        //}
                        //else
                        //{
                        //    await DisplayAlert("Error", "Error al guardar en la base de datos", "Ok");
                        //}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        await DisplayAlert("Error", "Ocurrió un error al procesar la grabación.", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "La grabación de audio ha fallado.", "Ok");
                }
                isRecording = false;
                btnGrabarAudio.Text = "Grabar Audio";
                Console.WriteLine("Deteniendo grabación y guardando el audio...");
            }
        }
        private void btnSalvarUbicacion_Clicked(object sender, EventArgs e)
        {

        }


    }
}