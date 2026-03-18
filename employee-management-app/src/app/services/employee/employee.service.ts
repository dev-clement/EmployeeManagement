import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from '../../../employee/employee';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private apiUrl = `${environment.apiUrl}/employee`;

  constructor(private http: HttpClient) {}

  /**
   * @brief Fetch all the employee coming from the
   * backend of the application as an observable
   * object
   * @returns An observable the user could subscribe to it
   */
  fetchEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }

  /**
   * @brief Fetch a single employee from the backend using its identifier
   * @param id The unique identifier of the employee to fetch
   * @returns An observable of the requested Employee
   */
  fetchEmployeeById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  /**
   * @brief Create an employee by sending it to the back-end
   * of the application
   * @param employee The employee being sent to the backend for
   * being saved
   * @returns An observable of Employee type to notify when
   * it's done
   */
  createEmployee(employee: Employee): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee);
  }

  /**
   * @brief Delete an employee from the data store using its identifier
   * @param id The unique identifier of the employee to delete
   * @returns An observable that notifies when the deletion is complete
   */
  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * @brief Update an existing employee in the data store
   * @param employee The employee object containing updated information
   * @returns An observable of the updated Employee
   */
  updateEmployee(employee: Employee): Observable<Employee> {
    return this.http.put<Employee>(`${this.apiUrl}/${employee.id}`, employee);
  }
}
