namespace PropertyManagerFL.UI.Services;

/// <summary>
/// Estado do tema partilhado entre o botão de alternância e os componentes
/// que precisam de reagir à mudança (por exemplo, gráficos Syncfusion,
/// que têm um enum Theme próprio, independente do CSS carregado).
/// </summary>
public sealed class ThemeStateService
{
    // O tema padrão é escuro; o script inline em _Layout.cshtml aplica-o antes do first paint.
    public bool IsDark { get; private set; } = true;

    public event Action? Changed;

    public void Set(bool isDark)
    {
        if (IsDark == isDark)
        {
            return;
        }

        IsDark = isDark;
        Changed?.Invoke();
    }
}
