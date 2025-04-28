import * as React from "react";
import { cn } from "../../lib/utils";
import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from "lucide-react";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  showPageNumbers?: boolean;
  pagesPerGroup?: number;
}

interface PaginatedTableProps extends React.HTMLAttributes<HTMLTableElement> {
  data: any[];
  itemsPerPage?: number;
  currentPage?: number;
  onPageChange?: (page: number) => void;
  totalItems?: number;
  showPageNumbers?: boolean;
  pagesPerGroup?: number;
}

const TablePagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange,
  showPageNumbers = true,
  pagesPerGroup = 5,
}) => {
  const getPageNumbers = () => {
    const pageNumbers = [];
    const halfPagesPerGroup = Math.floor(pagesPerGroup / 2);
    
    let startPage = Math.max(1, currentPage - halfPagesPerGroup);
    let endPage = Math.min(totalPages, startPage + pagesPerGroup - 1);
    
    if (endPage - startPage + 1 < pagesPerGroup) {
      startPage = Math.max(1, endPage - pagesPerGroup + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pageNumbers.push(i);
    }
    
    return pageNumbers;
  };
  
  return (
    <div className="flex items-center justify-end space-x-2 py-4">
      <button
        onClick={() => onPageChange(1)}
        disabled={currentPage === 1}
        className={cn(
          "flex h-8 w-8 items-center justify-center rounded-md border",
          currentPage === 1 
            ? "pointer-events-none opacity-50" 
            : "hover:bg-accent hover:text-accent-foreground"
        )}
      >
        <ChevronsLeft className="h-4 w-4" />
      </button>
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
        className={cn(
          "flex h-8 w-8 items-center justify-center rounded-md border",
          currentPage === 1 
            ? "pointer-events-none opacity-50" 
            : "hover:bg-accent hover:text-accent-foreground"
        )}
      >
        <ChevronLeft className="h-4 w-4" />
      </button>
      
      {showPageNumbers && getPageNumbers().map((page) => (
        <button
          key={page}
          onClick={() => onPageChange(page)}
          className={cn(
            "flex h-8 w-8 items-center justify-center rounded-md",
            currentPage === page
              ? "bg-primary text-primary-foreground"
              : "border hover:bg-accent hover:text-accent-foreground"
          )}
        >
          {page}
        </button>
      ))}
      
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
        className={cn(
          "flex h-8 w-8 items-center justify-center rounded-md border",
          currentPage === totalPages
            ? "pointer-events-none opacity-50"
            : "hover:bg-accent hover:text-accent-foreground"
        )}
      >
        <ChevronRight className="h-4 w-4" />
      </button>
      <button
        onClick={() => onPageChange(totalPages)}
        disabled={currentPage === totalPages}
        className={cn(
          "flex h-8 w-8 items-center justify-center rounded-md border",
          currentPage === totalPages 
            ? "pointer-events-none opacity-50" 
            : "hover:bg-accent hover:text-accent-foreground"
        )}
      >
        <ChevronsRight className="h-4 w-4" />
      </button>
    </div>
  );
};

const PaginatedTable = React.forwardRef<HTMLTableElement, PaginatedTableProps>(
  ({ className, data = [], itemsPerPage = 10, currentPage = 1, onPageChange, totalItems, showPageNumbers, pagesPerGroup, ...props }, ref) => {
    const [page, setPage] = React.useState(currentPage);
    const total = totalItems || data.length;
    const totalPages = Math.ceil(total / itemsPerPage);
    
    // Calculate paginated data
    const paginatedData = React.useMemo(() => {
      if (totalItems) {
        // Server-side pagination (data is already paginated)
        return data;
      } else {
        // Client-side pagination
        const startIndex = (page - 1) * itemsPerPage;
        return data.slice(startIndex, startIndex + itemsPerPage);
      }
    }, [data, page, itemsPerPage, totalItems]);
    
    // Handle page change
    const handlePageChange = React.useCallback((newPage: number) => {
      setPage(newPage);
      if (onPageChange) {
        onPageChange(newPage);
      }
    }, [onPageChange]);
    
    // Update local page state when currentPage prop changes
    React.useEffect(() => {
      setPage(currentPage);
    }, [currentPage]);
    
    return (
      <div className="w-full">
        <div className="relative w-full overflow-auto">
          <table ref={ref} className={cn("w-full caption-bottom text-sm", className)} {...props}>
            {props.children}
          </table>
        </div>
        
        {totalPages > 1 && (
          <TablePagination
            currentPage={page}
            totalPages={totalPages}
            onPageChange={handlePageChange}
            showPageNumbers={showPageNumbers}
            pagesPerGroup={pagesPerGroup}
          />
        )}
      </div>
    );
  }
);
PaginatedTable.displayName = "PaginatedTable";

const Table = React.forwardRef<HTMLTableElement, React.HTMLAttributes<HTMLTableElement>>(
  ({ className, ...props }, ref) => (
    <div className="relative w-full overflow-auto">
      <table ref={ref} className={cn("w-full caption-bottom text-sm", className)} {...props} />
    </div>
  ),
);
Table.displayName = "Table";

const TableHeader = React.forwardRef<HTMLTableSectionElement, React.HTMLAttributes<HTMLTableSectionElement>>(
  ({ className, ...props }, ref) => <thead ref={ref} className={cn("[&_tr]:border-b", className)} {...props} />,
);
TableHeader.displayName = "TableHeader";

const TableBody = React.forwardRef<HTMLTableSectionElement, React.HTMLAttributes<HTMLTableSectionElement>>(
  ({ className, ...props }, ref) => (
    <tbody ref={ref} className={cn("[&_tr:last-child]:border-0", className)} {...props} />
  ),
);
TableBody.displayName = "TableBody";

const TableFooter = React.forwardRef<HTMLTableSectionElement, React.HTMLAttributes<HTMLTableSectionElement>>(
  ({ className, ...props }, ref) => (
    <tfoot ref={ref} className={cn("bg-primary font-medium text-primary-foreground", className)} {...props} />
  ),
);
TableFooter.displayName = "TableFooter";

const TableRow = React.forwardRef<HTMLTableRowElement, React.HTMLAttributes<HTMLTableRowElement>>(
  ({ className, ...props }, ref) => (
    <tr
      ref={ref}
      className={cn("border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted", className)}
      {...props}
    />
  ),
);
TableRow.displayName = "TableRow";

const TableHead = React.forwardRef<HTMLTableCellElement, React.ThHTMLAttributes<HTMLTableCellElement>>(
  ({ className, ...props }, ref) => (
    <th
      ref={ref}
      className={cn(
        "h-12 px-4 text-left align-middle font-medium text-muted-foreground [&:has([role=checkbox])]:pr-0",
        className,
      )}
      {...props}
    />
  ),
);
TableHead.displayName = "TableHead";

const TableCell = React.forwardRef<HTMLTableCellElement, React.TdHTMLAttributes<HTMLTableCellElement>>(
  ({ className, ...props }, ref) => (
    <td ref={ref} className={cn("p-4 align-middle [&:has([role=checkbox])]:pr-0", className)} {...props} />
  ),
);
TableCell.displayName = "TableCell";

const TableCaption = React.forwardRef<HTMLTableCaptionElement, React.HTMLAttributes<HTMLTableCaptionElement>>(
  ({ className, ...props }, ref) => (
    <caption ref={ref} className={cn("mt-4 text-sm text-muted-foreground", className)} {...props} />
  ),
);
TableCaption.displayName = "TableCaption";

export { 
  Table, 
  PaginatedTable, 
  TableHeader, 
  TableBody, 
  TableFooter, 
  TableHead, 
  TableRow, 
  TableCell, 
  TableCaption 
};