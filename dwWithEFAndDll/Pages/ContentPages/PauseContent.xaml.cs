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
        // ���������� ��� �����������

        float step = 1; // ������� �������� ���
        double newValue = Math.Round(e.NewValue / step) * step; // ��������� �� ���������� ����

        SliderChangeWordTime.Value = newValue; // ������������� ����� �������� ��������

    }
}