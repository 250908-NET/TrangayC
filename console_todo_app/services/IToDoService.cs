public interface IToDoService
{
    void addItem(string item);
    string listItems();
    bool markItemComplete(int itemId);
    void deleteItem(int itemId);
    void exit();
}
