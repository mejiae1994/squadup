// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('.card').click(function (e) {
        e.preventDefault();
        var eventId = $(this).data('event-id');
        loadEventAttendance(eventId);
       
    });

    //because the button that triggers this is handle dynamically, we need to use event delegation from top down
    $(document).on('click', '#saveAttendanceButton', function (e) {
        e.preventDefault();
        updateEventAttendance();
    });


    function updateEventAttendance() {
        $("input[type=radio]:checked").each(function () {
            let memberId = $(this)[0].id
            let attendanceCode = $(this).val();
            let eventId = $(this).data("event-id");

            console.log(memberId);
            console.log(attendanceCode);
            console.log(eventId);
           
            $.ajax({
                url: "/squad/UpdateEventAttendance", 
                type: "POST", 
                data: { memberId: memberId, attendanceCode: attendanceCode, eventId: eventId },
                success: function (data) {
                    // Handle success, e.g., show a confirmation message
                    alert("Attendance updated successfully!");
                },
                error: function (error) {
                    
                    console.log(error);
                }
            });
        });
    }


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