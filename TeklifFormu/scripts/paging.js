$(function () {
    $(function () {
        $('#ExcelDiv').on('hover', function () {
            $(this).css('cursor', 'pointer');
        })
        $('#ExcelDiv').on('click', function () {
            $('<iframe src="/Home/GenerateExcel"></iframe>').appendTo('body').hide();
        });
    });
});

function getDetail(ID)
{
    window.open("Home/Order?CustomerID=" + ID, "_blank");
}

var _sortdir = 'asc';
var _preSort = '';

$(function () {
    //Sayfa ilk geldiginde yuklenir.
    $.getJSON("/Home/EfficientPaging", null, function (d) {
        // add the dynamic WebGrid to the body
        $("body").append(d.Data);

        // create the footer
        var footer = createFooter(d.Count);
        $("body").on("click", "#grid tfoot a", function (e) {
            e.preventDefault();
            var data = {
                page: $(this).text(),
                sort: _preSort, //En son sortlanan kolon ismi.
                sortdir: _sortdir == 'asc' ? 'desc' : 'asc' //En son gelen ne ise aynisini almka icin bir onceki sort sekline donuldu.
            };
            //Sayfalama buttonlarina basilinca paging yapilir.
            $.getJSON("/Home/EfficientPaging", data, function (html) {
                // add the data to the table
                $("#grid").remove();
                $("body").append(html.Data);

                // re-add the footer
                $('#grid thead').after(footer);

            });
        });

        //Tabloda kolonlar tiklaninca sorting islemi yapar
        $("body").on("click", "#grid thead th", function (e) {
            e.preventDefault();

            if (_sortdir == "desc" && $(this).text() != _preSort) { _sortdir = "asc"; }

            var data = {
                sort: $(this).text(),
                sortdir: _sortdir,
                presort: _preSort
            };
            _preSort = $(this).text();
            _sortdir = _sortdir == 'asc' ? 'desc' : 'asc';
            $.getJSON("/Home/EfficientSorting", data, function (html) {
                // add the data to the table
                $("#grid").remove();
                $("body").append(html.Data);

                // re-add the footer
                $('#grid thead').after(footer);

            });
        });
        //Tabloda kolonlarin ustune gelince mouse isareti cikartir.
        $("body").on({
            mouseenter: function () {
                $(this).css('cursor', 'pointer');
            },
            mouseleave: function () {
            }
        }, '#grid thead th');       

    });
});

function createFooter(d) {
    var rowsPerPage = 25;
    var footer = "<tfoot><tr class='webgrid-footer'><td colspan='7'>";
    for (var i = 1; i < (d + 1) ; i++) {
        footer = footer + "<a data-swhglnk='true' href=#><b>" + i + "</ b></a>&nbsp";
    }
    footer = footer + "</td></tr></tfoot>";
    $("#grid thead").after(footer);
    console.log(footer);
    return footer;
}