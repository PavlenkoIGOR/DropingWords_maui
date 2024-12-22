namespace dwWithEFAndDll.Pages.ContentPages;

public partial class PauseContent : ContentPage
{
	public float changeWordTime {  get; set; }
	public PauseContent()
	{
		InitializeComponent();
		//BindingContext = this;
	}
	public void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
        // Установите шаг перемещения

        float step = 1; // Задайте желаемый шаг
        double newValue = Math.Round(e.NewValue / step) * step; // Округляем до ближайшего шага

        SliderChangeWordTime.Value = newValue; // Устанавливаем новое значение ползунка

    }
}