//Create a function that will add an item to a list that is kept between function calls; so each function call will increase the size of the list. Upload your changes to your GitHub working branch.
let list = [];
function addItem(item) {
    list.push(item);
    return list;
}
addItem("Playing video games");
addItem("Watching movies or anime");
addItem("Working out");

function updateListFunction() {
    const itemInput = document.getElementById('itemInput');
    const newItem = itemInput.value;
    addItem(newItem);
    itemInput.value = '';   
}

function displayList() {
    const listElement = document.getElementById("myList");
    listElement.innerHTML = ""; 
    list.forEach(item => {
        const listItem = document.createElement("li");
        listItem.textContent = item;
        listElement.appendChild(listItem);
    });
}





