namespace Contract.Dto;

public class IdAndTitle
{
    public IdAndTitle(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id { get; set; }
    public string Title { get; set; }
}
