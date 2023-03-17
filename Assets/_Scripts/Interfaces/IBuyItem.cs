using Project.Inventory.Data;

namespace Project.Interfaces
{
    public interface IBuyItem
    {
        void BoughtItem(ItemSO itemSO);
        bool TrySpendCurrency(int currencyAmount);
    }
}