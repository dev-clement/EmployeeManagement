import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Employee } from '../../../employee/employee';
import { environment } from '../../../environments/environments';
import { EmployeeService } from './employee.service';

describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(EmployeeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('service should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch employees through GET', () => {
    const mockEmployees: Employee[] = [
      {
        id: 1,
        firstName: 'John',
        lastName: 'Doe',
        phone: '+33 6 33 33 33 33',
        position: 'Developper',
      },
    ];

    service.fetchEmployees().subscribe((employees) => {
      expect(employees.length).toBe(1);
      expect(employees).toEqual(mockEmployees);
    });

    // Check that the correct URL was called
    const req = httpMock.expectOne(`${environment.apiUrl}/employee`);
    expect(req.request.method).toBe('GET');

    // Resolve the request for future it
    req.flush(mockEmployees);
  });

  it('should fetch a single employee by ID through GET', () => {
    const mockEmployee: Employee = {
      id: 1,
      firstName: 'John',
      lastName: 'Doe',
      phone: '+33 6 33 33 33 33',
      position: 'Developer',
    };

    const employeeId = 1;

    service
      .fetchEmployeeById(employeeId)
      .subscribe((employee) => expect(employee).toEqual(mockEmployee));

    const req = httpMock.expectOne(`${environment.apiUrl}/employee/${employeeId}`);
    expect(req.request.method).toBe('GET');

    req.flush(mockEmployee);
  });

  it('should create an employee through POST', () => {
    const newEmployee: Employee = {
      id: 2,
      firstName: 'Jane',
      lastName: 'Smith',
      phone: '+33 6 12 34 56 78',
      position: 'Designer',
    };

    service.createEmployee(newEmployee).subscribe((response) => {
      expect(response).toEqual(newEmployee);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/employee`);

    // Check that it's a POST request
    expect(req.request.method).toBe('POST');

    // Verify the data sent in the body matches our input
    expect(req.request.body).toEqual(newEmployee);

    // Simulate the server returning the created employee
    req.flush(newEmployee);
  });

  it('should update an existing employee through PUT', () => {
    const updatedEmployee: Employee = {
      id: 1,
      firstName: 'John',
      lastName: 'Updated',
      phone: '+33 6 33 33 33 33',
      position: 'Senior Developper',
    };

    service.updateEmployee(updatedEmployee).subscribe((response) => {
      expect(response).toEqual(updatedEmployee);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/employee/${updatedEmployee.id}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatedEmployee);

    req.flush(updatedEmployee);
  });

  it('should handle error when updating an invalid employee', () => {
    const invalidEmployee: Employee = {
      id: 999, // ID that doesn't exist
      firstName: 'Ghost',
      lastName: 'User',
      phone: '000',
      position: 'N/A',
    };

    service.updateEmployee(invalidEmployee).subscribe({
      next: () => fail('Should have failed with a 404 error'),
      error: (error) => {
        expect(error.status).toBe(404);
      },
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/employee/${invalidEmployee.id}`);
    expect(req.request.method).toBe('PUT');

    // Simulate a 404 error from the server
    req.flush('Employee not found', { status: 404, statusText: 'Not Found' });
  });

  it('should delete an employee through DELETE', () => {
    const employeeId = 123;

    service.deleteEmployee(employeeId).subscribe((response) => {
      expect(response).toBeNull();
    });

    // Check that the URL includes the ID .../employee/123
    const req = httpMock.expectOne(`${environment.apiUrl}/employee/${employeeId}`);

    expect(req.request.method).toBe('DELETE');

    req.flush(null);
  });
});
