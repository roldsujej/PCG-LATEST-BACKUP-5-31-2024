// Show loading overlay when page starts loading
document.addEventListener('DOMContentLoaded', function () {
    var loadingOverlay = document.getElementById('loading-overlay');
    loadingOverlay.style.display = 'block'; // Show the loading overlay
});

// Hide loading overlay when page has fully loaded
window.addEventListener('load', function () {
    var loadingOverlay = document.getElementById('loading-overlay');
    loadingOverlay.style.display = 'none'; // Hide the loading overlay
});
