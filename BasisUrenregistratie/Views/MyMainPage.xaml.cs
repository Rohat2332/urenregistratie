using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasisUrenregistratie.Views;

public partial class MyMainPage : ContentPage
{
    public MyMainPage()
    {
        InitializeComponent();
    }
    
    void OnPage1Clicked(object sender,
        EventArgs e)
    {
        SubPage.Content = new Page1();
    }
    void OnPage2Clicked(object sender,
        EventArgs e)
    {
        SubPage.Content = new Page2();
    }
    
}