﻿namespace MauiPreview11Play;

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
		Application.Current.OpenWindow(new Window(new MainPage()));
	}

	private void AddOverlay(object sender, EventArgs e)
	{
		var window = GetParentWindow();
		var overlay = new TestWindowOverlay(window);
		window.AddOverlay(overlay);
	}
}

public class TestWindowOverlay : WindowOverlay
	{
		IWindowOverlayElement _testWindowDrawable;

		public TestWindowOverlay(Window window)
			: base(window)
		{
			_testWindowDrawable = new TestOverlayElement(this);

			AddWindowElement(_testWindowDrawable);

			EnableDrawableTouchHandling = true;
			Tapped += OnTapped;
		}

		async void OnTapped(object sender, WindowOverlayTappedEventArgs e)
		{
			if (!e.WindowOverlayElements.Contains(_testWindowDrawable))
				return;

			var window = Application.Current.Windows.FirstOrDefault(w => w == Window);

			System.Diagnostics.Debug.WriteLine($"Tapped the test overlay button.");

			var result = await window.Page.DisplayActionSheet(
				"Greetings from Visual Studio Client Experiences!",
				"Goodbye!",
				null,
				"Do something", "Do something else", "Do something... with feeling.");

			System.Diagnostics.Debug.WriteLine(result);
		}

		class TestOverlayElement : IWindowOverlayElement
		{
			readonly WindowOverlay _overlay;
			Circle _circle = new Circle(0, 0, 0);
			int _size = 20;

			public TestOverlayElement(WindowOverlay overlay)
			{
				_overlay = overlay;

				Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
				{
					_size += 10;
					_overlay.Invalidate();
					return true;
				});
			}

			public void Draw(ICanvas canvas, RectangleF dirtyRect)
			{
				canvas.FillColor = Color.FromRgba(255, 0, 0, 225);
				canvas.StrokeColor = Color.FromRgba(225, 0, 0, 225);
				canvas.FontColor = Colors.Orange;
				canvas.FontSize = 40f;

				var centerX = dirtyRect.Width / 2 - (_size/2);
				var centerY = dirtyRect.Height / 2 - (_size/2);
				_circle = new Circle(centerX, centerY, _size);

				canvas.FillCircle(centerX, centerY, _size);
				canvas.DrawString("🔥", centerX, centerY + 10, HorizontalAlignment.Center);
			}

			public bool Contains(Point point) =>
				_circle.ContainsPoint(new Point(point.X / _overlay.Density, point.Y / _overlay.Density));

			struct Circle
			{
				public float Radius;
				public PointF Center;

				public Circle(float x, float y, float r)
				{
					Radius = r;
					Center = new PointF(x, y);
				}

				public bool ContainsPoint(Point p) =>
					p.X <= Center.X + Radius &&
					p.X >= Center.X - Radius &&
					p.Y <= Center.Y + Radius &&
					p.Y >= Center.Y - Radius;
			}
		}
	}

