$(document).ready(function () {
    // Setup - add a text input to each footer cell
    var id = 0;
    $('#players tfoot th').each(function () {
        var initialValue = localStorage.getItem('columnSearcher' + id) || "";
        var title = $(this).text();
        $(this).html('<input type="text" class="columnSearchInput" placeholder="Search ' + title + '" id="columnSearcher' + id + '" value="' + initialValue + '"/>');
        id++;
    });

    var playerTable = $('#players').DataTable({
        stateSave: true,
        dom: 'Bfrtip',
        buttons: [
            "colvis",
            { extend: 'csv', exportOptions: { columns: ":visible" } },
            { extend: 'print', exportOptions: { columns: ":visible" } }
        ],
        columnDefs: [{
            targets: -1,
            searchable: false
        }]
    });

    // Apply the search
    playerTable.columns().every(function () {
        var that = this;

        $('input', this.footer()).on('keyup change', function () {
            if (that.search() !== this.value) {
                localStorage.setItem(this.id, this.value);
                if (this.id == "columnSearcher2") {
                    //This is the male/female column. Can't search for male unless you require the start of the string.
                    that.search('^' + this.value, true).draw();
                }
                else {
                    that.search(this.value, false).draw();
                }
            }
        });
    });
});
$(document).ready(function () {
    $('#teams').DataTable({
        stateSave: true
    });
});