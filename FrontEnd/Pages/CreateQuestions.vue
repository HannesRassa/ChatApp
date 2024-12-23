<template>
  <div class="flex-layout">
    <div class="create-pack-container">
      <!-- Home Button -->
      <button class="home-button" @click="goHome">Home</button>

      <h1>Create Question Pack</h1>

      <!-- Input for Pack Name -->
      <div class="form__group field">
        <input
        type="text"
        class="form__field"
        id="packName"
        v-model="packName"
        placeholder="Enter pack name"
        required
        />
        <label for="packName" class="form__label">Pack Name:</label>
      </div>

      <!-- Select Number of Questions -->
      <div class="form__group field">
        <input
        type="number"
        class="form__field"
        id="numberOfQuestions"
        v-model.number="numberOfQuestions"
        min="1"
        placeholder="Enter number of questions"
        @change="generateQuestionFields"
        />
        <label for="numberOfQuestions" class="form__label">Number of Questions:</label>
      </div>

      <!-- Dropdown for Question Inputs -->
      <div v-if="questions.length > 0" class="question-fields">
        <h3>Enter Questions:</h3>
        <div
          v-for="(question, index) in questions"
          :key="index"
          class="question-input"
        >
          <label :for="'question-' + index">Question {{ index + 1 }}:</label>
          <input
            :id="'question-' + index"
            type="text"
            v-model="question.questionText"
            placeholder="Enter question text"
            required
          />
        </div>
      </div>

      <!-- Save Button -->
      <button @click="submitQuestionPack" :disabled="isSubmitting">
        {{ isSubmitting ? "Saving..." : "Save Question Pack" }}
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

export default {
  name: "CreatePack",
  setup() {
    const packName = ref("");
    const numberOfQuestions = ref<number | null>(null);
    const questions = ref<{ questionText: string; packName: string }[]>([]);
    const isSubmitting = ref(false);
    const router = useRouter();

    const generateQuestionFields = () => {
      if (!numberOfQuestions.value || numberOfQuestions.value <= 0) {
        questions.value = [];
        return;
      }

      questions.value = Array.from({ length: numberOfQuestions.value }, () => ({
        questionText: "",
        packName: packName.value,
      }));
    };

    const submitQuestionPack = async () => {
      if (!packName.value.trim()) {
        alert("Pack name is required.");
        return;
      }

      if (questions.value.some((q) => !q.questionText.trim())) {
        alert("All question fields must be filled out.");
        return;
      }

      // Add pack name to each question
      questions.value.forEach((q) => (q.packName = packName.value.trim()));

      try {
        isSubmitting.value = true;
        const response = await axios.post(
          "https://localhost:7269/Backend/Question/Batch",
          questions.value
        );
        alert("Question pack saved successfully!");
        console.log(response.data);
        // Reset form
        packName.value = "";
        numberOfQuestions.value = null;
        questions.value = [];
      } catch (error) {
        console.error("Error saving question pack:", error);
        alert("Failed to save question pack.");
      } finally {
        isSubmitting.value = false;
      }
    };

    const goHome = () => {
      router.push("/");
    };

    return {
      packName,
      numberOfQuestions,
      questions,
      generateQuestionFields,
      submitQuestionPack,
      isSubmitting,
      goHome,
    };
  },
};
</script>

<style scoped>

.flex-layout {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: linear-gradient(135deg, #2c3e50, #4ca1af);
  padding: 20px;
  box-sizing: border-box;
}

.create-pack-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background: #d5d4d4;
  color: #34495e;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.page-title {
  font-size: 24px;
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.form-group {
  margin-bottom: 15px;
}

.form-label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #555;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
}

.primary-button {
  display: block;
  width: 100%;
  padding: 10px;
  font-size: 16px;
  font-weight: bold;
  color: #000;
  background-color: #a607ce;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.primary-button:disabled {
  background-color: #aaa;
  cursor: not-allowed;
}

.question-fields {
  margin-top: 20px;
}

.section-title {
  font-size: 18px;
  margin-bottom: 10px;
  color: #333;
}

.home-button {
  position: absolute;
  top: 10px;
  left: 10px;
  background-color: #34495e;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

.home-button:hover {
  background-color: #218f4f;
}

input {
  width: 100%;
  padding: 8px;
  margin: 8px 0;
  box-sizing: border-box;
}

h1 {
  margin-bottom: 20px;
}

.form__group {
  position: relative;
  padding: 20px 0 0;
  width: 100%;
  max-width: 180px;
}

.form__field {
  font-family: inherit;
  width: 100%;
  border: none;
  border-bottom: 2px solid #9b9b9b;
  outline: 0;
  font-size: 20px;
  color: #fff;
  padding: 7px 0;
  background: transparent;
  transition: border-color 0.2s;
}

.form__field::placeholder {
  color: transparent;
}

.form__field:placeholder-shown ~ .form__label {
  font-size: 17px;
  cursor: text;
  top: 35px;
}

.form__label {
  position: absolute;
  top: 0;
  display: block;
  transition: 0.2s;
  font-size: 17px;
  color: #9b9b9b;
  pointer-events: none;
}

.form__field:focus {
  padding-bottom: 6px;
  font-weight: 700;
  border-width: 3px;
  border-image: linear-gradient(to right, #116399, #38caef);
  border-image-slice: 1;
}

.form__field:focus ~ .form__label {
  position: absolute;
  top: 0;
  display: block;
  transition: 0.2s;
  font-size: 17px;
  color: #4ca1af;
  font-weight: 700;
}

/* reset input */
.form__field:required, .form__field:invalid {
  box-shadow: none;
}

</style>
