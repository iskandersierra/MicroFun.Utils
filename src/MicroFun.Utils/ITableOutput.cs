namespace MicroFun.Utils;

public interface ITableOutput
{
    public IEnumerable<string> GetHeaders();
    public IEnumerable<IEnumerable<string>> GetRows();
}
