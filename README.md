# Avalanche

Avalanche is a framework for Xamarin apps which tightly integrate with Rock Rms. It is designed for administrators who are used to administrating Rock, and developers who are used to developing Rock. Concepts like pages, blocks, lava and users translate 1 to 1 in the app. The goal is to create an app which is flexible and meets the needs of churches of all sizes.

## Getting Started

Avalanche is made of two parts. The Rock plugin and the app itself.

### Installing the Plugin (development)

Add the .csproj file to your Rock solution. There is a post build event which will move the dll into RockWeb/bin. There are two more folders which need to be installed. /Plugins/Avalanche will need to be installed into the RockWeb folder, and /Themes/Avalanche will need to be installed into the Themes folder. You may need to set up IIS Express to allow remote connections so that the app can talk to it. https://stackoverflow.com/questions/3313616/iis-express-enable-external-request

Make a new site using the Avalanche theme. Add a simple block to it (such as Label or Markdown) for testing. Go to General Settings > Global Attributes and put in the page number for the new page in the attribute Avalanche Home Page. You should now be able to go to http://localhost:6229/api/avalanche/home and see data.

### Installing the App

Open the solution in Visual Studio. Edit the file Utilities/Constants and point it to your environment. Debug in Android or iOS.

## Contributing

New features, wiki improvements or bug fixes welcome! Check out the [wiki](https://github.com/secc/Avalanche/wiki)
 for more information. 
 
[![Build status](https://build.appcenter.ms/v0.1/apps/58278ba1-767b-474b-ba5b-228a3629a64e/branches/master/badge)](https://appcenter.ms)
 
## License

Distributed under the Southeast Christian Church License Agreement. Plugins and other includes under their respective liscences.

[Markdown View](https://github.com/aloisdeniel/MarkdownView) used under MIT license 
