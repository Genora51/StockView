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
            var printDialog = new PrintDialog();
            var flowDocument = new FlowDocument();
            var table = CreateTable(data, summaryData, title);
            flowDocument.Blocks.Add(table);
            const int padding = 50;
            flowDocument.PagePadding = new Thickness(padding);
            flowDocument.ColumnGap = 0;
            flowDocument.ColumnWidth = printDialog.PrintableAreaWidth - padding * 2;
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource doc = flowDocument;
                printDialog.PrintDocument(doc.DocumentPaginator, title);
            }
            
        }

        private Table CreateTable(DataTable data, DataTable summaryData, string title)
        {
            var table = new Table()
            {
                BorderThickness = GlobalFormating.TableBorderThickness,
                BorderBrush = GlobalFormating.TableBorderBrush,
                FontFamily = new FontFamily("Calibri"),
                CellSpacing = 0
            };

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

            var sepRow = new TableRow();
            var sepCell = new TableCell
            {
                FontSize = 0.1,
                BorderThickness = new Thickness(0, 0.3, 0, 0.3),
                BorderBrush = Brushes.Black,
                ColumnSpan = data.Columns.Count
            };
            sepRow.Cells.Add(sepCell);
            dataRowGroup.Rows.Add(sepRow);

            for (var i = 0; i < summaryData.Rows.Count; i ++)
            {
                var row = new TableRow()
                {
                    FontWeight = GlobalFormating.TableFooterFontWeight,
                    FontSize = GlobalFormating.TableFooterFontSize,
                    Background = GlobalFormating.TableFooterBackground
                };
                var dataRow = summaryData.Rows[i];
                foreach (var column in summaryData.Columns.Cast<DataColumn>())
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new TextBlock
                    {
                        Text = dataRow[column].ToString(),
                        TextWrapping = TextWrapping.NoWrap
                    });
                    if (column.ColumnName != "Statistic")
                    {
                        paragraph.TextAlignment = TextAlignment.Right;
                    }
                    row.Cells.Add(CreateCell(paragraph));
                }
                dataRowGroup.Rows.Add(row);
            }
            return table;
        }

        private TableRow CreateTitleRow(DataTable data, string title)
        {
            // Title Row
            var row = new TableRow
            {
                Background = GlobalFormating.TableTitleBackground,
                FontSize = GlobalFormating.TableTitleFontSize,
                FontWeight = GlobalFormating.TableTitleFontWeight
            };
            var titleCell = new TableCell(new Paragraph(new Run(title)))
            {
                Padding = new Thickness(GlobalFormating.TableCellSpacing)
            };
            titleCell.ColumnSpan = data.Columns.Count;
            row.Cells.Add(titleCell);
            return row;
        }

        private TableRow CreateHeaderRow(DataTable data)
        {
            TableRow row;
            // Column headers
            row = new TableRow
            {
                FontSize = GlobalFormating.ColumnsHeaderRowFontSize,
                FontWeight = GlobalFormating.ColumnsHeaderRowFontWeight
            };
            for (var i = 0; i < data.Columns.Count; i++)
            {
                var column = data.Columns[i];
                row.Cells.Add(
                    CreateCell(
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
                FontSize = GlobalFormating.DataRowsFontSize,
                FontWeight = GlobalFormating.DataRowsFontWeight
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
                    row.Cells.Add(CreateCell(
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
            return CreateCell(
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
                paragraph.Inlines.Add(new Run(" xd"));
            }
            return CreateCell(paragraph);
        }

        private TableCell CreateCell(Block block)
        {
            return new TableCell(block)
            {
                BorderThickness = new Thickness(0.3),
                Padding = new Thickness(GlobalFormating.TableCellSpacing),
                BorderBrush = Brushes.LightGray
            };
        }
    }

    public class GlobalFormating
    {
        // -------------
        public static double TableCellSpacing { get; set; } = 5;
        public static Thickness TableBorderThickness { get; set; } = new Thickness() { Top = 1, Left = 1, Bottom = 1, Right = 1 };
        public static Brush TableBorderBrush { get; set; } = Brushes.Black;
        public static Brush TableTitleBackground { get; set; } = Brushes.Silver;
        public static double TableTitleFontSize { get; set; } = 18;
        public static FontWeight TableTitleFontWeight { get; set; } = FontWeights.Bold;
        // -------------
        public static Brush TableFooterBackground { get; set; } = Brushes.White;
        public static double TableFooterFontSize { get; set; } = 11;
        public static FontWeight TableFooterFontWeight { get; set; } = FontWeights.Normal;
        // -------------
        public static double ColumnsHeaderRowFontSize { get; set; } = 14;
        public static FontWeight ColumnsHeaderRowFontWeight { get; set; } = FontWeights.Bold;
        // -------------
        public static double DataRowsFontSize { get; set; } = 12;
        public static FontWeight DataRowsFontWeight { get; set; } = FontWeights.Normal;
    }
}
