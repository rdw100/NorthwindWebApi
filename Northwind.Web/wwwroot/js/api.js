var url = "https://localhost:44314/api/customers";

var customersList = document.getElementById("customers-list");
if (customersList) {
    fetch(url)
        .then(response => response.json())
        .then(data => showCustomers(data))
        .catch(ex => {
            alert("Unable to retrieve data ...");
            console.log(ex);
        });
}

function showCustomers(customers)
{
    customers.forEach(c => {
        let li = document.createElement("li");
        let text = `${c.companyName} (${c.customerId})`;
        li.appendChild(document.createTextNode(text));
        customersList.appendChild(li);
    });
}