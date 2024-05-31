$(document).ready(function () {
    // Event delegation for dynamically added elements
    $(".menu").on("click", "ul > li", function (e) {
        // Check if the clicked element has a sub-menu
        if ($(this).find("ul").length > 0) {
            e.preventDefault(); // Prevent default anchor behavior only if it has a sub-menu
            $(this).toggleClass("active");
            $(this).find("ul").slideToggle();
            // Close other sub menus
            $(this).siblings().removeClass("active").find("ul").slideUp();
            // Remove active class of sub menu items
            $(this).siblings().find("ul li").removeClass("active");
        }
    });

    // Stop propagation of click events on sub-menu items
    $(".menu").on("click", ".sub-menu li", function (e) {
        e.stopPropagation();
    });

    $(".menu-btn").click(function () {
        $(".sidebar").toggleClass("active");
    });

    function toggleSidebar() {
        $(".smallMenu-btn").click(function () {
            $(".sidebar").toggleClass("active");
        });
    }
});

// Get the modal
var modal = document.getElementById("customModal");

// Get the button that opens the modal
var forwardButtons = document.querySelectorAll(".contactButton");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on the button, open the modal
forwardButtons.forEach(function (button) {
    button.addEventListener("click", function () {
        modal.style.display = "block";
    });
});

// When the user clicks on <span> (x), close the modal
function closeModal() {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
};


