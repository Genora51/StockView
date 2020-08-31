using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace StockView.UI.View.Services
{
    public class PrintService : IPrintService
    {
        public void Print(DataTable data, DataTable summaryData, string title)
        {
            var flowDocument = new FlowDocument();
            var table = CreateTable(data, summaryData, title);
            flowDocument.Blocks.Add(table);
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource doc = flowDocument;
                printDialog.PrintDocument(doc.DocumentPaginator, title);
            }
            
        }

        private Table CreateTable(DataTable data, DataTable summaryData, string title)
        {
            var table = new Table();

            // TODO: global formatting
            for (var x = 0; x < data.Columns.Count; x++)
            {
                table.Columns.Add(new TableColumn());
            }

            var dataRowGroup = new TableRowGroup();
            table.RowGroups.Add(dataRowGroup);
            dataRowGroup.Rows.Add(CreateTitleRow(data, title));
            dataRowGroup.Rows.Add(CreateHeaderRow(data));
            for (var i = 0; i < data.Rows.Count; i++)
            {
                var row = data.Rows[i];
                var prevRow = i > 0 ? data.Rows[i - 1] : null;
                dataRowGroup.Rows.Add(CreateDataRow(row, prevRow));
            }
            // TODO: summaries
            return table;
        }

        private static TableRow CreateTitleRow(DataTable data, string title)
        {
            // Title Row
            var row = new TableRow
            {
                // TODO: formatting
            };
            var titleCell = new TableCell(
                new Paragraph(
                    new Run(title)
                )
            );
            titleCell.ColumnSpan = data.Columns.Count;
            row.Cells.Add(titleCell);
            return row;
        }

        private static TableRow CreateHeaderRow(DataTable data)
        {
            TableRow row;
            // Column headers
            row = new TableRow
            {
                // Formatting
            };
            for (var i = 0; i < data.Columns.Count; i++)
            {
                var column = data.Columns[i];
                row.Cells.Add(
                    new TableCell(
                        new Paragraph(
                            new Run(column.ColumnName)
                        )
                    )
                );
            }

            return row;
        }

        private TableRow CreateDataRow(DataRow dataRow, DataRow prevRow)
        {
            var row = new TableRow
            {
                // Formatting
            };
            foreach (var column in dataRow.Table.Columns.Cast<DataColumn>())
            {
                var obj = dataRow[column];
                if (obj is DateTime date)
                {
                    row.Cells.Add(CreateDateCell(date));
                }
                else if (obj is StockSnapshotWrapper wrapper)
                {
                    row.Cells.Add(CreateSnapshotCell(wrapper, prevRow?[column] as StockSnapshotWrapper));
                } else if (obj is DBNull) {
                    row.Cells.Add(new TableCell(
                        new Paragraph(new Run("-"))
                        {
                            TextAlignment = TextAlignment.Center
                        }
                    ));
                }
            }
            return row;
        }

        private TableCell CreateDateCell(DateTime date)
        {
            var text = date.ToString("dd-MMM");
            return new TableCell(
                new Paragraph(
                    new Run(text)
                )
            );
        }

        private TableCell CreateSnapshotCell(StockSnapshotWrapper snapshot, StockSnapshotWrapper prevSnap)
        {
            var prevVal = prevSnap?.Value;
            var valueRun = new Run(snapshot.Value.ToString("N2"))
            {
                Foreground = prevVal.HasValue && snapshot.Value < prevVal.Value ? Brushes.Red : Brushes.Black,
                FontWeight = prevVal.HasValue && snapshot.Value > prevVal.Value ? FontWeights.Bold : FontWeights.Normal
            };

            var paragraph = new Paragraph(valueRun)
            {
                TextAlignment = TextAlignment.Right
            };
            if (snapshot.ExDividends)
            {
                paragraph.Inlines.Add(new Run("xd"));
            }
            return new TableCell(paragraph);
        }
    }
}
