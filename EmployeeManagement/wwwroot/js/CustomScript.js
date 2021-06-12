function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = "deleteSpan_" + uniqueId;
    var confirmDeleteSpan = "confirmDeleteSpan_" + uniqueId;

    if (isDeleteClicked) {
        $('#' + confirmDeleteSpan).show();
        $('#' + deleteSpan).hide();
    }
    else {
        $('#' + confirmDeleteSpan).hide();
        $('#' + deleteSpan).show();
    }
}