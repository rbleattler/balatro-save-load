global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Maui;
global using Microsoft.Maui.Controls;
global using Microsoft.Maui.Hosting;
global using Microsoft.Maui.Graphics;
global using Microsoft.Maui.Storage;

global using CommunityToolkit.Maui;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using BalatroSaveToolkit.Models;
global using BalatroSaveToolkit.Services.Interfaces;
global using BalatroSaveToolkit.Services.Implementations;
global using BalatroSaveToolkit.ViewModels;
global using BalatroSaveToolkit.Views;

// Platform detection helpers
// These will be uncommented when platform-specific code is implemented
//#if WINDOWS
//global using BalatroSaveToolkit.Platforms.Windows;
//#elif MACCATALYST
//global using BalatroSaveToolkit.Platforms.MacCatalyst;
//#elif LINUX
//global using BalatroSaveToolkit.Platforms.Linux;
//#endif
