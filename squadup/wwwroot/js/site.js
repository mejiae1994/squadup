// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $(document).on('input', '#eventPrice', function (e) {
        e.preventDefault();
        if ($(this).val() > 0) {
            $('#isSplitPrice').prop('disabled', false);
        } else {
            $('#isSplitPrice').prop('disabled', true);
        }
    });

    //because the button that triggers this is handle dynamically, we need to use event delegation from top down
    $(document).on('click', '#saveAttendanceButton', function (e) {
        e.preventDefault();
        updateEventAttendance();
    });
    $(document).on('click', '#deleteSquadEvent', function (e) {
        e.preventDefault();
        var eventId = $(this).data('event-id');
        deleteSquadEvent(eventId);
    });

    $(document).on('click', '#eventCard', function (e) {
        e.preventDefault();
        var eventId = $(this).data('event-id');
        loadEventAttendance(eventId);
    });

    $(document).on('click', '#copyUrlButton', function (e) {
        e.preventDefault();
        var urlToCopy = window.location.href;
        navigator.clipboard.writeText(urlToCopy);
    });

    $(document).on('click', '#deleteSquadMember', function (e) {
        e.preventDefault();
        var memberId = $(this).data('member-id');
        console.log(memberId);
        deleteSquadMember(memberId);
    });

    $(document).on('click', '#addSquadMember', function (e) {
        e.preventDefault();
        let squdaId = $(this).data('squad-id');
        let squadMember = $('#squadMemberInput').val().trim();
        if (squadMember !== '') {
            addSquadMember(squdaId, squadMember);
            $('#squadMemberInput').val('');
        }
        else {
            alert("enter squad member name")
        }
    });

    function updateEventAttendance() {
        let attendanceList = [];

        $("input[type=radio]:checked").each(function () {
            let memberId = $(this)[0].id;
            let attendanceCode = $(this).val();
            let eventId = $(this).data("event-id");

            attendanceList.push({ memberId, attendanceCode, eventId });
        });

        $.ajax({
            url: "/squad/UpdateEventAttendance",
            type: "POST",
            data: { attendanceUpdates: attendanceList },
            success: function (data) {
                $("#squadEventListContainer").html(data);
                showToast();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

    function loadEventAttendance(eventId) {
        $.ajax({
            url: "/squad/GetEventAttendance?eventId=" + eventId,
            type: "GET",
            success: function (data) {
                $("#eventAttendanceContainer").html(data);
                $("#eventAttendanceModal").modal("show");
            },
            error: function (error) {
                // Handle errors if needed
                console.log(error);
            }
        });
    }

    function deleteSquadEvent(eventId) {
        $.ajax({
            url: "/squad/DeleteSquadEvent",
            type: "POST",
            data: { eventId: eventId },
            success: function (data) {
                $("#squadEventListContainer").html(data);
            },
            error: function (error) {
                // Handle errors if needed
                console.log(error);
            }
        });
    }

    function addSquadMember(squadId, squadMember) {
        $.ajax({
            url: "/squad/AddSquadMember",
            type: "POST",
            data: { squadId: squadId, squadMember: squadMember},
            success: function (data) {
                // Populate the modal body with the data
                if (data.success) {
                    console.log('true')
                    window.location.reload()
                }
                else {
                    alert(data.message)
                }
            },
            error: function (error) {
                // Handle errors if needed
                console.log(error);
            }
        });
        var toastLiveExample = document.getElementById('liveToast')
        var toast = new bootstrap.Toast(toastLiveExample)
        toast.show()
    }

    function deleteSquadMember(memberId) {
        $.ajax({
            url: "/squad/DeleteSquadMember",
            type: "POST",
            data: { memberId: memberId},
            success: function (data) {
                // Populate the modal body with the data
                if (data.success) {
                    console.log('true')
                    window.location.reload()
                }
                else {
                    alert(data.message)
                }
            },
            error: function (error) {
                // Handle errors if needed
                console.log(error);
            }
        });
    }

    function showToast() {
        var toastLiveExample = document.getElementById('liveToast')
        var toast = new bootstrap.Toast(toastLiveExample)
        toast.show()
    }
});