using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalanche.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avalanche.Blocks
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentView
	{
		public Login ()
		{
			InitializeComponent ();
		}

        private void btnSubmit_Clicked( object sender, EventArgs e )
        {
            RockClient.Login( username.Text, password.Text );
        }
    }
}