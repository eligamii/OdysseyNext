namespace Odyssey.Shared.Enums
{

    //##################### PINS #################################
    public enum PinShowMode
    {
        OnPane, // to be shown in the "Pinned" tabs section
        OnSearchBar // to be shown on the search bar, when nothing in the searchbar's textBox
    }





    //##################### QUICKACTIONSITEMS #####################

    public enum QuickActionShowType
    {
        None,
        ContextMenuItem,
        MainMoreMenuFlyout
    }

    public enum QuickActionShowPosition
    {
        Top,
        InShowMoreOptions,
        BeforeInspectItem
    }

    public enum QuickActionShowCondition
    {
        None,
        HasLinkText,
        HasLinkUri,
        HasSelection,
        HasSourceUri,
        IsEditable
    }

}
