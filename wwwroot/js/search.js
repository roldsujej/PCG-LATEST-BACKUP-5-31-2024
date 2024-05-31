// Define a global variable to store the original display state of each row
var originalDisplayStates = [];

function searchTable() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("search");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    tr = table.getElementsByTagName("tr");

    // If originalDisplayStates array is empty, populate it with the current display states of all rows
    if (originalDisplayStates.length === 0) {
        for (i = 0; i < tr.length; i++) {
            originalDisplayStates[i] = tr[i].style.display || "";
        }
    }

    // Iterate through all table rows
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        var found = false;
        // Iterate through all cells of the current row
        for (var j = 0; j < td.length; j++) {
            if (td[j]) {
                txtValue = td[j].textContent || td[j].innerText;
                // Check if the cell content matches the search filter
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
        }
        // Show/hide rows based on the search results
        if (found) {
            tr[i].style.display = originalDisplayStates[i]; // Restore original display state
        } else {
            tr[i].style.display = "none";
        }
    }

    // Reapply pagination logic
    applyPagination();
}
