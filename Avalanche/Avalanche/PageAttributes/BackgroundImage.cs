using Xamarin.Forms;

namespace Avalanche.PageAttributes
{
    class BackgroundImage : IPageAttribute
    {
        public void Modify( ContentPage contentPage, string value )
        {
            var mainGrid = contentPage.FindByName<Grid>( "MainGrid" );
            if ( mainGrid != null )
            {
                var image = new Image()
                {
                    Aspect = Aspect.AspectFill,
                    Source = value

                };
                mainGrid.Children.Add( image );
            }
        }
    }
}
