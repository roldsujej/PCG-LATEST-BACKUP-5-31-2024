//document.addEventListener("DOMContentLoaded", function () {
//    console.log("Document loaded...");

//    // Function for toggling dropdown
//    function toggleDropdown(event) {
//        const dropdownContainer = event.target.closest(".searchable-dropdown");
//        if (!dropdownContainer) return; // Exit if clicked outside dropdown

//        const dropdown = dropdownContainer.querySelector(".dropdown-content");
//        const caret = dropdownContainer.querySelector(".caret");
//        const inputField = dropdownContainer.querySelector(".search-input");

//        if (event.target === caret) {
//            dropdown.classList.toggle("show");
//        } else if (!event.target.closest(".dropdown-content") && event.target !== inputField) {
//            dropdown.classList.remove("show");
//        }
//    }

//    // Function for selecting dropdown options
//    function selectDropdownOption(event) {
//        const dropdownOption = event.target.closest(".dropdown-content li");
//        if (!dropdownOption) return; // Exit if clicked outside options

//        const dropdownContainer = dropdownOption.closest(".searchable-dropdown");
//        const inputField = dropdownContainer.querySelector(".search-input");
//        inputField.value = dropdownOption.textContent;
//        dropdownContainer.querySelector(".dropdown-content").classList.remove("show");
//    }

//    // Function for filtering dropdown options
//    function filterDropdownOptions(inputField) {
//        const filter = inputField.value.toLowerCase();
//        const dropdownOptions = inputField.closest(".searchable-dropdown").querySelectorAll(".dropdown-content li");
//        dropdownOptions.forEach(option => {
//            const text = option.textContent.toLowerCase();
//            if (text.includes(filter)) {
//                option.style.display = "block";
//            } else {
//                option.style.display = "none";
//            }
//        });
//    }




//    // Event delegation for toggling dropdown
//    document.addEventListener("click", toggleDropdown);

//    // Event delegation for selecting dropdown options
//    document.addEventListener("click", selectDropdownOption);

//    // Event listener for input changes in search input fields
//    document.querySelectorAll(".search-input").forEach(inputField => {
//        inputField.addEventListener("input", function () {
//            console.log("Input changed...");
//            filterDropdownOptions(inputField);
//        });
//    });
//});