import { Component } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { System } from '../models/system';
import { CommonModule } from '@angular/common';
import { AccountService } from '../services/account.service';
import { SystemService } from '../services/system.service';
import { LoadingComponent } from '../loading/loading.component';
import { DisableControlDirective } from '../directives/disabled-control.directive';
import { v4 as uuid } from 'uuid'
import { Subject, interval, takeUntil } from 'rxjs';

@Component({
  selector: 'app-system',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingComponent, DisableControlDirective],
  templateUrl: './system.component.html',
  styleUrl: './system.component.css'
})
export class SystemComponent {
  parameters: number[] = [];
  roots: number[] | null = [];
  postSystemForm: FormGroup;
  postParametersForm: FormGroup;
  isPostSystemFormSubmitted: boolean = false;
  rows: number = 3;
  postRowsForm: FormGroup;
  isPostParametersSubmitted: boolean = false;
  systems: System[] = [];
  putSystemForm: FormGroup;
  loading: boolean = false;

  isInputValid: boolean = true;

  editId: string | null = null;

  tasksInRun: number = 0;
  maxTasks: number = 2;
  destroy$ = new Subject<void>();


  constructor(private systemService: SystemService, private accountService: AccountService) {
    this.postSystemForm = new FormGroup({
      parameters: new FormArray([])
    });

    this.postParametersForm = new FormGroup({
      parameters: new FormArray([]),
      roots: new FormArray([]),
    });

    this.postRowsForm = new FormGroup({
      rows: new FormControl(null)
    });

    this.putSystemForm = new FormGroup({
      systems: new FormArray([])
    })
  }

  get postRows_RowsControl(): any {
    return this.postRowsForm.controls['rows'];
  }
  get postParametersFormArray(): FormArray {
    return this.postParametersForm.get("parameters") as FormArray;
  }

  get postRootsFormArray(): FormArray {
    return this.postParametersForm.get("roots") as FormArray;
  }

  public resizePostForm(): void {
    this.rows = this.postRows_RowsControl.value;

    //console.log(`start this.parameters: ${this.parameters}`);

    if (this.parameters.length < this.rows) {
      let length = this.parameters.length;
      for (var i = length; i < this.rows; i++) {
        this.parameters.push(0);
      }
    }
    else {
      while (this.parameters.length != this.rows) {
        this.parameters.pop();
      }
    }

    this.postParametersFormArray.clear();
    this.parameters.forEach((param) => {
      this.postParametersFormArray.push(new FormGroup({
        value: new FormControl(param, [Validators.required, Validators.pattern("^-?[0-9]*\.?[0-9]+$")])
      }));
    });

    //console.log(`end this.parameters ${this.parameters}`);
  }

  public async postParametersSubmitted() {
    console.log(this.tasksInRun);
    this.isPostSystemFormSubmitted = true;
    if (this.tasksInRun >= this.maxTasks) {
      alert(`maximum number of tasks was reached, unable to start more tasks`);
      console.log("alert reached");
      return;
    }
    this.parameters = [];
    this.loading = true;
    this.isInputValid = true;

    for (let i = 0; i < this.postParametersFormArray.length; i++) {
      if (this.postParametersFormArray.at(i).invalid) {
        this.isInputValid = false;
        return;
      }
    }
    this.tasksInRun++;

    for (let i = 0; i < this.postParametersFormArray.length; i++) {
      let element = this.postParametersFormArray.at(i).value.value as number;
      this.parameters.push(element);
    }

    for (let i = 0; i < this.postParametersFormArray.length; i++) {
      this.postParametersFormArray.controls[i].reset(this.postParametersFormArray.controls[i].value);
    }
    let system = new System(uuid(), this.parameters, null);

    this.postSystemSubmitted(system);
  }
  
  get putSystemFormArray(): FormArray {
    return this.putSystemForm.get("systems") as FormArray;
  }

  loadSystems() {
    this.systemService.getSystems().subscribe({
      next: (response: System[]) => {
        this.systems = response;

        var parameters = [];
        for (var i = 0; i < this.systems.length; i++) {
          var str = '';
          var currentLength = this.systems.at(i)?.parameters?.length;
          if (currentLength != undefined) {
            for (var j = 0; j < currentLength; j++) {
               
            }
          }
        }
        this.putSystemFormArray.clear();

        this.systems.forEach(system => {
          this.putSystemFormArray.push(new FormGroup({
            id: new FormControl(system.id, [Validators.required]),
            parameters: new FormArray([]),
          }));
        });

        console.log(this.systems);

        this.systems.forEach((system: System, ind) => {
          system.parameters?.forEach((param) => {
            this.putSystem_ParametersControl(ind).push(new FormGroup({
              value: new FormControl(param, [Validators.required])
          }))});
        });
        console.log(this.putSystemFormArray);
      },

      error: (error: any) => {
        console.log(error);
      },

      complete: () => { }
    });
  }
  ngOnInit() {
    this.refreshClicked();
    this.loadSystems();
  }

  // removed postCity_NameControl
  public putSystem_ParametersControl(i: number): FormArray {
    let currentFormGroup = this.putSystemFormArray.controls[i] as FormGroup;
    return currentFormGroup.controls['parameters'] as FormArray;
  }

  public postSystemSubmitted(system: System) {
    this.systems.push(system);
    
    this.putSystemFormArray.push(new FormGroup({
        id: new FormControl(system.id, [Validators.required]),
        parameters: new FormArray([]),
      }));
    
    system.parameters?.forEach((param) => {
      this.putSystem_ParametersControl(this.systems.length - 1).push(new FormGroup({
          value: new FormControl(param, [Validators.required])
        }))
    });

    //this.postSystemForm.value
    this.systemService.postSystem(system).subscribe({
      next: (response: string | null) => {
        this.isInputValid = true;
        if (system.id != null) {
          this.monitorProgress(system.id);
        }
      },

      error: (error: any) => {
        console.log(error);
        this.isInputValid = false;
      },
      complete: () => { }
    });
  }

  private getTaskProgress(system: System): void {
    this.systemService.getTaskProgress(system.id).subscribe({

     next: completed => {
      let isCompleted = completed

      if (isCompleted === true) {
        this.tasksInRun--;
        this.loadSystems();
        system.destroy$.next();
        system.destroy$.complete();
        }
      },
      error: (error: any) => {
        system.destroy$.next();
        system.destroy$.complete();
        console.log(error);
      },
      complete: () => {

      }
    });
  }

  private monitorProgress(id: string): void {
    let system = this.systems.find(x => x.id === id);
    if (system === undefined) {
      alert("Some error happened");
      return;
    }
      
    this.getTaskProgress(system);
    interval(1000)
      .pipe(takeUntil(system.destroy$))
      .subscribe({
        next: () => {
          if (system == undefined) {
            return;
          }
          this.getTaskProgress(system);
        },
        error: (message: any) => {
          system?.destroy$.next();
          console.log(message);
        },
        complete: () => {
        }
      });
  }

  public editClicked(System: System) {
    this.editId = System.id;
  }

  public deleteClicked(system: System, i: number): void {
    if (confirm(`Are you sure to delete this System: ${system.parameters}?`)) {
      this.systemService.deleteSystem(system.id).subscribe({
        next: (response: string) => {
          console.log(response);
          //this.editId = null;

          this.putSystemFormArray.removeAt(i);
          this.systems.splice(i, 1);
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => { }
      })
    }
  }

  public terminateClicked(system: System, i: number): void {
    if (confirm(`Are you sure to terminate this System: ${system.parameters}?`)) {
      this.systemService.terminateSystem(system.id).subscribe({
        next: (response: string) => {
          console.log(`terminate response: ${response}`);
          this.tasksInRun--;

          this.putSystemFormArray.removeAt(i);
          this.systems.splice(i, 1);
        },
        error: (error: any) => {
          console.log(error);
        },
        complete: () => { }
      })
    }
  }

  refreshClicked(): void {
    this.accountService.postGenerateNewToken().subscribe({
      next: (response: any) => {
        localStorage["token"] = response.token;
        localStorage["refreshToken"] = response.refreshToken;

      },
      error: (error: any) => {
        console.log(error);
      },
      complete: () => { }
    });
  }

  public checkRoots(i: number): void {
    this.roots = this.systems.at(i)?.roots as number[];
    
    this.postRootsFormArray.clear();
    

    if (this.roots != null) {
      this.roots.forEach((param) => {
        this.postRootsFormArray.push(new FormGroup({
          value: new FormControl(param, [Validators.required])
        }));
      });
    }
  }
}
