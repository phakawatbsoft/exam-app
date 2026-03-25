import { Component, inject, OnInit, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Quiz } from './services/quiz';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';
import { switchMap } from 'rxjs/operators';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-root',
  imports: [ButtonModule, ReactiveFormsModule, InputTextModule, RadioButtonModule, CardModule ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly sv = inject(Quiz);
  private readonly fb = inject(FormBuilder);
  protected readonly title = signal('frontend');

  questions: any[] = [];
  answer: any = null;
  examForm!: FormGroup;
  score: number = 0;

  ngOnInit(): void {
    // 1. สร้าง Form เริ่มต้น
    this.examForm = this.fb.group({
      fullName: ['', Validators.required],
      answers: this.fb.group({}) // Group สำหรับเก็บคำตอบแต่ละข้อ
    });

    this.sv.getQuestions().pipe(
      switchMap((questionsData: any[]) => {
        const answersGroup = this.examForm.get('answers') as FormGroup;

        // 2. จัดการข้อมูลคำถามและสร้าง Form Controls
        this.questions = questionsData.map(q => {
          answersGroup.addControl(q.id.toString(), this.fb.control('', Validators.required));
          return {
            ...q,
            parsedOptions: q.options.split(',')
          };
        });
        console.log('โหลดข้อมูลคำถามสำเร็จ:', this.questions);

        // 3. เมื่อสร้างฟอร์มเสร็จแล้ว ให้ไปดึงข้อมูลคำตอบล่าสุด
        return this.sv.getResults();
      })
    ).subscribe((results: any[]) => {
      if (results && results.length > 0) {
        // 4. นำข้อมูลล่าสุด (สมมติว่าเป็นตัวสุดท้าย) มา patch ลงฟอร์ม
        const latestResult = results[results.length - 1];
        console.log('โหลดข้อมูลคำตอบล่าสุด:', latestResult);

        if (latestResult && latestResult.answers) {
          try {
            const savedAnswers = JSON.parse(latestResult.answers);
            this.examForm.patchValue({
              fullName: latestResult.fullName,
              answers: savedAnswers
            });
            
            this.answer = true;
            this.score = latestResult.score;
            this.examForm.disable();
          } catch (e) {
            console.error('Failed to parse saved answers', e);
          }
        }
      }
    });
  }

  submitExam() {
    if (this.examForm.valid) {
      const formValue = this.examForm.value
      const payload = {
        fullName: formValue.fullName,
        // แปลง answers { "1": "3", ... } ให้เป็น String '{"1":"3", ...}'
        answers: JSON.stringify(formValue.answers) 
      };
      console.log('ข้อมูลที่ต้องการส่ง:', payload);
      
      // ยิง API ส่งคำตอบไปที่ Backend
      this.sv.submitResult(payload).pipe(
        switchMap((res :any) => this.sv.getResultById(res.id))
      ).subscribe({
        next: (res) => {
          console.log('ส่งข้อสอบสำเร็จ!', res);
          this.examForm.patchValue({fullName: res.fullName, answers: JSON.parse(res.answers)})
          this.answer = true; // เปลี่ยนเป็นหน้า "สอบอีกครั้ง" เมื่อส่งสำเร็จ
          this.score = res.score;
          this.examForm.disable();
        },
        error: (err) => console.error('เกิดข้อผิดพลาดในการส่งข้อสอบ', err)
      });
    } else {
      this.examForm.markAllAsTouched(); // เพื่อให้แจ้งเตือนฟิลด์ที่ยังไม่กรอก
    }
  }

  retakeExam() {
    this.examForm.reset();
    this.examForm.enable();
    this.answer = null;
    this.score = 0;
  }
}