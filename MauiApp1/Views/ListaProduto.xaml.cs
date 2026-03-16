using MauiApp1.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Views;  // ← era MauiAppMinhasCompras, corrigido para MauiApp1

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        lista.Clear(); // ← limpa antes de repopular para não duplicar ao voltar da tela
        List<Produto> tmp = await App.Db.GetAll();
        tmp.ForEach(i => lista.Add(i));
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        string q = e.NewTextValue;

        lista.Clear();

        if (string.IsNullOrWhiteSpace(q))
        {
            // se apagou tudo, volta a lista completa
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        else
        {
            List<Produto> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i));
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);
        DisplayAlert("Total dos Produtos", $"O total é {soma:C}", "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        MenuItem item = sender as MenuItem;
        Produto produto = item.CommandParameter as Produto;

        if (produto != null)
        {
            await App.Db.Delete(produto.Id);
            lista.Remove(produto);
        }
    }
}