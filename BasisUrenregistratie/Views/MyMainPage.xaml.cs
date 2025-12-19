using System;
using Microsoft.Maui.Controls;

namespace BasisUrenregistratie.Views;

public partial class MyMainPage : ContentPage
{
    public MyMainPage()
    {
        InitializeComponent();
    }

    void OnPage1Clicked(object sender, EventArgs e) => SubPage.Content = new Page1();
    void OnPage2Clicked(object sender, EventArgs e) => SubPage.Content = new Page2();
    void OnFormClicked(object sender, EventArgs e) => SubPage.Content = new FormPage();
}