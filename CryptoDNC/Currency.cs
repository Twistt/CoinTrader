public class Currency
{
    public string Symbol {get;set;}
    public decimal Balance {get;set;}
    public ConsolePosition SymbolPos {get;set;}
    public ConsolePosition BalancePos {get;set;}
}
public struct ConsolePosition {

    public int top;
    public int left;
}