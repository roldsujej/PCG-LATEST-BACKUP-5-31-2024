
    var currentPage = 1;
    var rowsPerPage = 5; // Number of rows per page
    var maxPageNumbers = 5; // Maximum number of page numbers to display

    function showPage(page) {
        var tableRows = document.getElementById("myTable").rows;
        var totalRows = tableRows.length;

        // Calculate start and end indexes for the current page
        var startIndex = (page - 1) * rowsPerPage;
        var endIndex = Math.min(startIndex + rowsPerPage, totalRows);

        // Hide all rows
        for (var i = 0; i < totalRows; i++) {
            tableRows[i].style.display = "none";
        }

        // Show rows for the current page
        for (var i = startIndex; i < endIndex; i++) {
            tableRows[i].style.display = "";
        }

        // Update page numbers
        updatePageNumbers(page);
    }

    function prevPage() {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
        }
    }

    function nextPage() {
        var tableRows = document.getElementById("myTable").rows;
        var totalRows = tableRows.length;

        if (currentPage < Math.ceil(totalRows / rowsPerPage)) {
            currentPage++;
            showPage(currentPage);
        }
    }

    // Update page numbers
    function updatePageNumbers(currentPage) {
        var tableRows = document.getElementById("myTable").rows;
        var totalRows = tableRows.length;
        var totalPages = Math.ceil(totalRows / rowsPerPage);

        var pageNumbersHTML = "";
        var startPage = Math.max(1, currentPage - Math.floor(maxPageNumbers / 2));
        var endPage = Math.min(totalPages, startPage + maxPageNumbers - 1);

        if (endPage - startPage + 1 < maxPageNumbers) {
            startPage = Math.max(1, endPage - maxPageNumbers + 1);
        }

        for (var i = startPage; i <= endPage; i++) {
            pageNumbersHTML += "<li><a href='#' onclick='showPage(" + i + ")' " + (i === currentPage ? "class='active'" : "") + ">" + i + "</a></li>";
        }

        document.getElementById("pageNumbers").innerHTML = pageNumbersHTML;
    }

    // Initially show the first page
    showPage(currentPage);

