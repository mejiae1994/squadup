// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Attach a click event handler to each event card
    $('.card').click(function (e) {
        // Get the event ID from the clicked card using .data('event-id')
        e.preventDefault();
        var eventId = $(this).data('event-id');
        loadEventAttendance(eventId);
       
    });


    function loadEventAttendance(eventId) {
        
        $.ajax({
            url: "/squad/GetEventAttendance?eventId=" + eventId,
            type: "GET",
            success: function (data) {
                // Populate the modal body with the data

                // Open the modal
                $("#eventAttendanceContainer").html(data);
                $("#eventAttendanceModal").modal("show");

            },
            error: function (error) {
                // Handle errors if needed
                console.log(error);
            }
        });
    }
});