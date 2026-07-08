import Box from '@mui/material/Box';
import { DataGrid, type DataGridProps, type GridValidRowModel } from '@mui/x-data-grid';

export interface DataTableProps<R extends GridValidRowModel = GridValidRowModel>
  extends Omit<DataGridProps<R>, 'autoHeight'> {
  /** Fixed height in px when not using auto-height row-based sizing. Default 480. */
  height?: number;
}

/**
 * Thin, opinionated wrapper around MUI X DataGrid with SpeededERP defaults
 * (density, pagination, disabled cell selection) so product pages don't
 * repeat the same boilerplate.
 */
export function DataTable<R extends GridValidRowModel = GridValidRowModel>({
  height = 480,
  density = 'comfortable',
  pageSizeOptions = [10, 25, 50],
  disableRowSelectionOnClick = true,
  initialState,
  ...props
}: DataTableProps<R>) {
  const firstOption = pageSizeOptions[0];
  const defaultPageSize = typeof firstOption === 'number' ? firstOption : (firstOption?.value ?? 10);

  return (
    <Box sx={{ width: '100%', height }}>
      <DataGrid<R>
        density={density}
        pageSizeOptions={pageSizeOptions}
        disableRowSelectionOnClick={disableRowSelectionOnClick}
        initialState={{
          pagination: { paginationModel: { pageSize: defaultPageSize } },
          ...initialState,
        }}
        {...props}
      />
    </Box>
  );
}
