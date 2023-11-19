using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;

using Microsoft.Maui.Maps;
using Plugin.Maui.Audio;


namespace PM2Examen2Grupo1
{
    public partial class MainPage : ContentPage
    {
        string videoPath;

        //Audio
        private readonly IAudioRecorder _audioRecorder;
        private bool isRecording = false;
        public string pathaudio, filename;
        public MainPage()
        {
            InitializeComponent();
            _audioRecorder = AudioManager.Current.CreateRecorder();
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    var connection = Connectivity.NetworkAccess;
        //    //var local = CrossGeolocator.Current;

        //    if (connection == NetworkAccess.Internet)
        //    {
        //        if (local == null || !local.IsGeolocationAvailable || !local.IsGeolocationEnabled)
        //        {
        //            // Si la geolocalización no está disponible o no está habilitada
        //            await CheckAndRequestLocationPermissionAsync();
        //        }
        //        else
        //        {
        //            await GetLocationAsync();
        //        }
        //    }
        //    else
        //    {
        //        await DisplayAlert("Sin Acceso a internet", "Por favor, revisa tu conexion a internet para continuar.", "OK");
        //    }
        //}

        //private async Task CheckAndRequestLocationPermissionAsync()
        //{
        //    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        //    if (status != PermissionStatus.Granted)
        //    {
        //        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //    }

        //    if (status == PermissionStatus.Granted)
        //    {
        //        await GetLocationAsync();
        //    }
        //    else if (status == PermissionStatus.Denied)
        //    {
        //        await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        //    }
        //}
        //public async Task GetLocationAsync()
        //{
        //    try
        //    {
        //        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
        //        var location = await Geolocation.GetLocationAsync(request);

        //        if (location != null)
        //        {
        //            txtLatitud.Text = "" + location.Latitude;
        //            txtLongitud.Text = "" + location.Longitude;
        //        }

        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        await DisplayAlert("Advertencia", fnsEx + "", "OK");
        //    }
        //    catch (PermissionException Ex)
        //    {
        //        await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Advertencia", ex + "", "OK");
        //    }
        //}




        private void OnCounterClicked(object sender, EventArgs e)
        {
           
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {

        }

        //public async Task<bool> CheckAndRequestStoragePermission()
        //{
        //    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        //    if (status != PermissionStatus.Granted)
        //    {
        //        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        //    }

        //    return status == PermissionStatus.Granted;
        //}

        public async Task<bool> CheckAndRequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
                if (status != PermissionStatus.Granted)
                {
                    // Permiso denegado
                    return false;
                }
            }
            // Permiso otorgado
            return true;
        }


        private async void btnGabarVideo_Clicked(object sender, EventArgs e)
            {
            //if (await CheckAndRequestPermissionAsync<Permissions.StorageWrite>())
            //{
                var videoOptions = new MediaPickerOptions
                {
                    Title = "Capturar video"

                };

                var video = await MediaPicker.CaptureVideoAsync(videoOptions);

                if (video != null)
                {
                    using (var stream = await video.OpenReadAsync())
                    {
                        videoPath = GetVideoPath(stream);

                        viewVideo.Source = MediaSource.FromUri(new Uri(videoPath));
                    }
                }
            //}
            //else
            //{
            //    await DisplayAlert("Alerta", "No tiene los permisos","ok");
            //}

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
    }
}