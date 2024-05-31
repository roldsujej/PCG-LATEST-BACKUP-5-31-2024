  document.addEventListener("DOMContentLoaded", function () {
        const dropdowns = document.querySelectorAll('.searchable-dropdown');

        dropdowns.forEach(dropdown => {
            const inputField = dropdown.querySelector('.search-input');
            const dropdownContent = dropdown.querySelector('.dropdown-content');

            inputField.addEventListener("focus", function () {
                dropdownContent.style.display = "block";
            });

            inputField.addEventListener("input", function () {
                filterDropdown(inputField.id, dropdownContent.id);
            });

            document.addEventListener("click", function (event) {
                if (!event.target.closest(".searchable-dropdown")) {
                    dropdownContent.style.display = "none";
                }
            });

            dropdownContent.addEventListener("click", function (event) {
                const li = event.target.closest("li");
                if (li) {
                    inputField.value = li.textContent;
                    dropdownContent.style.display = "none";
                }
            });
        });
    });

function filterDropdown(inputId, dropdownId) {
    var input, filter, ul, li, i, txtValue;
    input = document.getElementById(inputId);
    filter = input.value.toLowerCase();
    ul = document.querySelector(`#${dropdownId} ul`);
    li = ul.getElementsByTagName("li");

    let anyVisible = false;

    for (i = 0; i < li.length; i++) {
        txtValue = li[i].textContent || li[i].innerText;
        if (txtValue.toLowerCase().indexOf(filter) > -1) {
            li[i].style.display = "";
            anyVisible = true;
        } else {
            li[i].style.display = "none";
        }
    }

    let noResultsItem = ul.querySelector('.no-results');

    if (!anyVisible) {
        if (!noResultsItem) {
            noResultsItem = document.createElement('li');
            noResultsItem.textContent = "No results found";
            noResultsItem.classList.add('no-results');
            noResultsItem.style.color = '#red';
            noResultsItem.style.fontStyle = 'italic';
            ul.appendChild(noResultsItem);
        } else {
            noResultsItem.style.display = "";
        }
    } else {
        if (noResultsItem) {
            noResultsItem.style.display = "none";
        }
    }
}


    function toggleDropdown(dropdownId) {
        var dropdownContent = document.getElementById(dropdownId);
        if (dropdownContent.style.display === "block") {
            dropdownContent.style.display = "none";
        } else {
            dropdownContent.style.display = "block";
        }
    }