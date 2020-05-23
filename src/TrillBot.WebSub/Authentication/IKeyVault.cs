namespace TrillBot.WebSub.Authentication
{
    internal interface IKeyVault
    {
        byte[] GetKeyAsBytes();
        string GetKey();
    }
}