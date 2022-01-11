namespace MauiPreview11Play;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;
		CounterLabel.Text = $"Current count: {count}";

		SemanticScreenReader.Announce(CounterLabel.Text);
	}

	private void CloseWindow(object sender, EventArgs e)
	{
		var window = this.GetParentWindow();
		if (window is not null)
			Application.Current.CloseWindow(window);
	}

	private void OpenWindow(object sender, EventArgs e)
	{
		var win = new Window(new MainPage());
		win.AddOverlay(new WindowOverlay(win));
		Application.Current.OpenWindow(win);
	}
}

