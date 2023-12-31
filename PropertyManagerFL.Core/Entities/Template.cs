namespace PropertyManagerFL.Core.Entities;
public  class Template
{
    public int Id { get; set; }
    public string?FileName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
