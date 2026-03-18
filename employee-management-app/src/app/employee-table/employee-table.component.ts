import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Employee } from '../../employee/employee';
import { EmployeeService } from '../services/employee/employee.service';

@Component({
  selector: 'employee-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employee-table.component.html',
  styleUrl: './employee-table.component.css',
})
export class EmployeeTableComponent {
  employees: Employee[] = [];

  constructor(
    private employeeService: EmployeeService,
    private router: Router
  ) {}

  ngOnInit() {
    this.employeeService.fetchEmployees().subscribe((employees: Employee[]) => {
      this.employees = employees;
    });
  }

  editEmployee(id: number): void {
    this.router.navigate(['/edit', id]);
  }

  deleteEmployee(id: number): void {
    this.employeeService.deleteEmployee(id).subscribe({
      next: () => {
        this.employees = this.employees.filter((e) => e.id !== id);
      },
      error: (err) => {
        console.error(`Error deleting employee: ${err.message}`);
      },
    });
  }
}
