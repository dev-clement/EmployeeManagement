import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Employee } from '../../employee/employee';
import { EmployeeTableComponent } from './employee-table.component';
import { EmployeeService } from '../services/employee/employee.service';
import { of } from 'rxjs';

describe('EmployeeTableComponent', () => {
  let component: EmployeeTableComponent;
  let fixture: ComponentFixture<EmployeeTableComponent>;
  let employeesServiceSpy: jasmine.SpyObj<EmployeeService>;

  const mockEmployees: Employee[] = [
    { id: 1, firstName: 'Clement', lastName: 'Dev', phone: '+33 6 33 33 33 33', position: 'Developper' }
  ]

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('EmployeeService', ['fetchEmployees']);

    spy.fetchEmployees.and.returnValue(of(mockEmployees));

    await TestBed.configureTestingModule({
      imports: [EmployeeTableComponent]
      , providers: [
        { provide: EmployeeService, useValue: spy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmployeeTableComponent);
    component = fixture.componentInstance;
    employeesServiceSpy = TestBed.inject(EmployeeService) as jasmine.SpyObj<EmployeeService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call fetchEmployees on init and populate the employees array', () => {
    // Trigger the ngOnInit hook
    fixture.detectChanges();

    // Check if the service has been called
    expect(employeesServiceSpy.fetchEmployees).toHaveBeenCalled();

    // Verify the component state has been updated
    expect(component.employees.length).toBe(1);
    expect(component.employees).toEqual(mockEmployees);
  });
});
