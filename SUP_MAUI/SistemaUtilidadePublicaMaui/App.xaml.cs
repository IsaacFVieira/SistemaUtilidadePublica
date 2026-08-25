using Microsoft.Extensions.DependencyInjection;
using SistemaUtilidadePublicaMaui.Features.Authentication.Registro.Views;

namespace SistemaUtilidadePublicaMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new Registar());
        }
    }
}