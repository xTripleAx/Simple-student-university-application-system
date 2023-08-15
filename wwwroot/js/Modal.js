function openModal() {
    $('#ApprovingConfirm').modal('show');
}

$(document).ready(function () {
    $('#approveButton').on('click', function () {
        openModal();
    });

    $('#disapproveButton').on('click', function () {
        openModal();
    });
});